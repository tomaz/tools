####################################################
#
# SRT subtitle files tweaking
#
# Prerequisites:
#
# gem install rchardet
# gem install srt
#
####################################################

require 'optparse'
require 'rchardet'
require 'time'
require 'srt'

# options hash that will be filled out while parsing
$options = {}

# the OptionParser object
$options_parser

####################################################
# Command line options
####################################################

# initializes `$options_parser` and parses command line into `$options`, leaving arguments in `ARGV`
def parse_command_line
	puts "Parsing command line"

	# options parser definition
	$options_parser = OptionParser.new do |opts|
		opts.banner = "Usage: ruby srt_converter.rb [options] path"

		opts.on("-s TIME", "--start TIME", "Original start time `HH:MM:SS,mmm`") do |v|
			$options[:start_original] = convert_timestamp_to_float(v)
		end

		opts.on("-d TIME", "--start-new TIME", "New start time `HH:MM:SS,mmm`") do |v|
			$options[:start_new] = convert_timestamp_to_float(v)
		end

		opts.on("-e TIME", "--end TIME", "Original end time `HH:MM:SS,mmm`") do |v|
			$options[:end_original] = convert_timestamp_to_float(v)
		end

		opts.on("-r TIME", "--end-new TIME", "New end time `HH:MM:SS,mmm`") do |v|
			$options[:end_new] = convert_timestamp_to_float(v)
		end

		opts.on("-o ENC", "--encoding ENC", "Encoding `windows-1250`, `iso-8859-2`...") do |v|
			$options[:encoding] = v
		end

		opts.on("-h", "--help", "Print this help") do
			puts opts
			exit
		end
	end

	# parse the options; this will result in `$options` to fill in with all command line options and `ARGV` with arguments
	$options_parser.parse!

	# if none of the new times is provided, exit
	if !($options.has_key?(:start_new) || !$options.has_key?(:end_new))
		puts help_string
		puts "ERROR: either new time for start or end is required (--start-new or --end_new)"
		exit
	end

	# if path to source file is not provided, print help and exit
	if ARGV.empty?
		puts help_string
		puts "ERROR: missing source srt file"
		exit
	end

	if ARGV.length > 1
		puts "Converting #{ARGV.length} SRT files"
	end
end

# prints help string from `$options_parser`
def help_string
	$options_parser.help
end

####################################################
# Values convesion
####################################################

# converts given timestamp from string to float
def convert_timestamp_to_float(timestamp)
	formatted_time = "#{timestamp.sub(",", ".")}"
	time = Time.parse(formatted_time)
	(time.hour * 3600 + time.min * 60 + time.sec).to_f + time.usec.fdiv(1000000)
end

def convert_float_to_timestamp(value)
	milliseconds = (value * 1000.0).to_i

	hours = milliseconds / 3600000
	milliseconds -= hours * 3600000

	minutes = milliseconds / 60000
	milliseconds -= minutes * 60000

	seconds = milliseconds / 1000
	milliseconds -= seconds * 1000

	return "#{hours.to_s.rjust(2, "0")}:#{minutes.to_s.rjust(2, "0")}:#{seconds.to_s.rjust(2, "0")},#{milliseconds.to_s.rjust(3, "0")}"
end

####################################################
# SRT parsing
####################################################

# parses given SRT file and returns hash that contains description of the file:
#
# - `:srt` = SRT::File
# - `:encoding` = encoding to be used on output file
# - `:source_path` = expanded file name with full path
# - `:destination_path` = expanded file name for destination file with full path (may or may not yet exist)
# - `:destination_filename` = filename and extension for destination
# - `:start_sequence` = sequence number corresponding to starting line
# - `:start_original_time` = original time of the starting line in seconds as float
# - `:start_new_time` = new time of the starting line in seconds as float (for shifting)
# - `:end_sequence` = sequence number corresponding to ending line
# - `:end_original_time` = original time of the ending line in seconds as float
# - `:end_new_time` = new time of the ending line in seconds as float (for shifting)
def parse_srt_file(path)
	# if file doesn't exist, print help and exit
	if !File.file?(path)
		puts $options_parser.help
		exit
	end

	result = {}

	# prepare full path and log the file
	expanded_path = File.expand_path(path)
	filename = File.basename(expanded_path)
	puts "Converting #{filename}"

	# open the file and optionally detect encoding; note it's much more accurate to open with known encoding!
	contents =
		if $options.has_key?(:encoding)
			forced_encoding = $options[:encoding]
			result[:encoding] = forced_encoding
			puts "Using encoding from arguments (#{forced_encoding})"
			File.read(expanded_path, :encoding => forced_encoding)
		else
			contents = File.read(expanded_path)
			detected_encoding = CharDet.detect(contents)
			puts "Detected encoding #{detected_encoding['encoding']} (confidence #{(detected_encoding['confidence'] * 100).round(0)}%)"
			contents
		end

	# prepare destination filename
	source_path = File.dirname(expanded_path)
	source_extension = File.extname(expanded_path)
	source_filename = File.basename(expanded_path, source_extension)
	destination_filename = File.join(source_path, "#{source_filename}-shifted#{source_extension}")

	# read contents
	srt = SRT::File.parse(contents)

	# if original start time not provided, get it from file
	if $options.has_key?(:start_original)
		result[:start_original_time] = $options[:start_original]
		puts "Using original start time from arguments (#{convert_float_to_timestamp($options[:start_original])})"
	else
		start_from_file = srt.lines.first.start_time
		result[:start_original_time] = start_from_file
		puts "Using original start time from file (#{convert_float_to_timestamp(start_from_file)})"
	end

	# if new start time is not provided, get it from file
	if $options.has_key?(:start_new)
		result[:start_new_time] = $options[:start_new]
		puts "Using new start time from arguments (#{convert_float_to_timestamp($options[:start_new])})"
	else
		start_from_file = srt.lines.first.start_time
		result[:start_new_time] = start_from_file
		puts "Using new start time from file (#{convert_float_to_timestamp(start_from_file)})"
	end

	# if original end time not provided, get it from file
	if $options.has_key?(:end_original)
		result[:end_original_time] = $options[:end_original]
		puts "Using original end time from arguments (#{convert_float_to_timestamp($options[:end_original])})"
	else
		end_from_file = srt.lines.last.start_time
		result[:end_original_time] = end_from_file
		puts "Using original end time from file (#{convert_float_to_timestamp(end_from_file)})"
	end

	# if new end time not provided, get it from file
	if $options.has_key?(:end_new)
		result[:end_new_time] = $options[:end_new]
		puts "Using new end time from arguments (#{convert_float_to_timestamp($options[:end_new])})"
	else
		end_from_file = srt.lines.last.start_time
		result[:end_new_time] = end_from_file
		puts "Using new end time from file (#{convert_float_to_timestamp(end_from_file)})"
	end

	# now that we know our start times, we can find the corresponding sequence numbers
	srt.lines.each do |line|
		if line.start_time == result[:start_original_time]
			puts "Found original start time at sequence ##{line.sequence}"
			result[:start_sequence] = line.sequence
		elsif line.start_time == result[:end_original_time]
			puts "Found original ending time at sequence ##{line.sequence}"
			result[:end_sequence] = line.sequence
		end
	end

	# if we didn't find both sequences show error and exit
	if !result.has_key?(:start_sequence)
		puts "ERROR: Start time #{convert_float_to_timestamp(result[:start_original_time])} not found in file!"
		exit
	end

	if !result.has_key?(:end_sequence)
		puts "ERROR: End time #{convert_float_to_timestamp(result[:end_original_time])} not found in file!"
		exit
	end

	# return the hash with data
	result[:srt] = srt
	result[:source_path] = expanded_path
	result[:destination_path] = destination_filename
	result[:destination_filename] = File.basename(destination_filename)
	result
end

# parses all SRT files from `ARGV`
def parse_srt_files
	ARGV.each do |file|
		puts ""
		description = parse_srt_file file
		timeshift description
		save_results description
	end

	puts ""
	puts "DONE!"
end

# timeshifts source file to destination; see `parse_srt_file` for details on hash keys
def timeshift(file)
	srt = file[:srt]

	start_sequence = "##{file[:start_sequence]}"
	start_new_time = file[:start_new_time]
	end_sequence = "##{file[:end_sequence]}"
	end_new_time = file[:end_new_time]

	puts "Timeshifting #{start_sequence} #{convert_float_to_timestamp(start_new_time)} -> #{end_sequence} #{convert_float_to_timestamp(end_new_time)}"

	srt.timeshift({ start_sequence => start_new_time, end_sequence => end_new_time })
end

# saves timeshifted file into destination; see `parse_srt_file` for details on hash keys
def save_results(file)
	puts "Saving to #{file[:destination_filename]}"

	contents = file[:srt].to_s
	contents.force_encoding(file[:encoding])

	File.write(file[:destination_path], contents)
end

####################################################
# Main program
####################################################

parse_command_line
parse_srt_files
