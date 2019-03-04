require 'date'
require 'securerandom'

####################################################
# declarations
####################################################

class Event
    def initialize(data)
        times = data[1].split('-')
        @from = DateTime.strptime("#{data[0]} #{times.first}", "%Y%m%d %H%M")
        @to = DateTime.strptime("#{data[0]} #{times.last}", "%Y%m%d %H%M")
        @title = data[2]
        @location = data[3]
    end

    def now
        "#{DateTime.now.strftime('%FT%T')}Z"
    end

    def from_time
        "#{@from.strftime('%FT%T')}Z"
    end

    def to_time
        "#{@to.strftime('%FT%T')}Z"
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
output << "BEGIN:VCALENDAR\r\n"
output << "PRODID:-//Tomaz Kragelj//Calendar//EN\r\n"
output << "VERSION:2.0\r\n"
output << "CALSCALE:GREGORIAN\r\n"
output << "METHOD:PUBLISH\r\n"
output << "X-WR-CALNAME:\(arguments.calendarName)\r\n"
output << "X-WR-TIMEZONE:UTC\r\n"
output << "X-WR-CALDESC:\r\n"

events.each do |event|
    uuid = SecureRandom.uuid.gsub('-','')
    puts "- #{event.description}"
    output << "BEGIN:VEVENT\r\n"
    output << "DTSTART:#{event.from_time}\r\n"
    output << "DTEND:#{event.to_time}\r\n"
    output << "DTSTAMP:#{event.from_time}\r\n"
    output << "UID:#{uuid}@gentlebytes.com\r\n"
    output << "CREATED:#{event.now}\r\n"
    output << "DESCRIPTION:\r\n"
    output << "LAST-MODIFIED:#{event.now}\r\n"
    output << "LOCATION:#{event.location}\r\n"
    output << "SEQUENCE:0\r\n"
    output << "STATUS:CONFIRMED\r\n"
    output << "SUMMARY:#{event.title}\r\n"
    output << "TRANSP:TRANSPARENT\r\n"
    output << "END:VEVENT\r\n"
end

output << "END:VCALENDAR\r\n"
File.write(arg_dest_file, output.string)

exit(0)
