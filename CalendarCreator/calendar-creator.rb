# gem install chronic
# gem install tzinfo
# gem install tzinfo-data

require 'date'
require 'chronic'
require 'securerandom'

####################################################
# declarations
####################################################

class Event
    def initialize(data)
        # Convert date from `20200510` to `2020-05-10` format
        date = data[0]
        formatted_date = "#{date[0..3]}-#{date[4..5]}-#{date[6..7]}"

        # get both times, from & to
        times = data[1].split('-')

        @from = Chronic.parse("#{formatted_date} #{times.first}")
        @to = Chronic.parse("#{formatted_date} #{times.last}")
        puts "#{@from}"
        @title = data[2]
        @location = data[3]
    end

    def now
        "#{DateTime.now.strftime('%FT%T')}"
    end

    def from_time
        "#{@from.strftime('%FT%T')}"
    end

    def to_time
        "#{@to.strftime('%FT%T')}"
    end

    def title
        @title
    end

    def location
        @location
    end

    def description
        "#{@from.strftime('%d %b %H:%M')}-#{@to.strftime('%H:%M')} #{@title}"
    end
end

####################################################
# setup defaults
####################################################

arg_calendar_name = "Racing"
arg_source_file = ""
arg_dest_file = ""

####################################################
# read arguments
####################################################

ARGV.each do |a|
    arg_source_file = a
end

if arg_source_file.empty?
    puts "USAGE: ruby calendar-creator.rb <input_file>"
    exit(1)
end

arg_path = File.dirname(arg_source_file)
if (arg_path == ".")
    arg_path = ""
end

arg_dest_file = 
    arg_path + 
    File.basename(arg_source_file, File.extname(arg_source_file)) + 
    ".ics"

puts "Parameters:"
puts "- Calendar '#{arg_calendar_name}s'"
puts "- Source '#{arg_source_file}'"
puts "- Destination '#{arg_dest_file}'"

####################################################
# read contents of the file
####################################################

events = Array.new
data = Array.new

puts "Reading:"
File.open(arg_source_file).each do |line|
    trimmed = line.strip

    next if trimmed.empty?

    data.push(trimmed)
    
    if (data.count == 4)
        events.push(Event.new(data))
        puts "- #{events.last.description}"
        data.clear
    end
end

if !data.empty?
    puts "INVALID DATA #{data}"
    exit(1)
end

####################################################
# create ics file
####################################################

puts "Generating"

output = StringIO.new
output << "BEGIN:VCALENDAR\n"
output << "PRODID:-//Tomaz Kragelj//Calendar//EN\n"
output << "VERSION:2.0\n"
output << "CALSCALE:GREGORIAN\n"
output << "METHOD:PUBLISH\n"
output << "X-WR-CALNAME:\(arguments.calendarName)\n"
output << "X-WR-TIMEZONE:UTC\n"
output << "X-WR-CALDESC:\n"

events.each do |event|
    uuid = SecureRandom.uuid.gsub('-','')
    puts "- #{event.description}"
    output << "BEGIN:VEVENT\n"
    output << "DTSTART:#{event.from_time}\n"
    output << "DTEND:#{event.to_time}\n"
    output << "DTSTAMP:#{event.from_time}\n"
    output << "UID:#{uuid}@gentlebytes.com\n"
    output << "CREATED:#{event.now}\n"
    output << "DESCRIPTION:\n"
    output << "LAST-MODIFIED:#{event.now}\n"
    output << "LOCATION:#{event.location}\n"
    output << "SEQUENCE:0\n"
    output << "STATUS:CONFIRMED\n"
    output << "SUMMARY:#{event.title}\n"
    output << "TRANSP:TRANSPARENT\n"
    output << "END:VEVENT\n"
end

output << "END:VCALENDAR\n"
File.write(arg_dest_file, output.string)

exit(0)
