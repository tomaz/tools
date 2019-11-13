require 'tty-prompt'
require 'pastel'
require 'bigdecimal'

class BigDecimal
	def to_string
		if frac == BigDecimal(0)
			return to_i.to_s
		else
			return to_s("F")
		end
	end
end

class Formatting

	def cls
		system("clear") || system("cls")
	end

	def put_title(title)
		puts
		puts $pastel.bold.underline.red(title)
		puts
	end

	def option(key, name)
		"(#{$pastel.bold.bright_yellow(key)}) #{name}"
	end
end

class Program

	def run
		$format.cls

		$format.put_title "M A T E M A T I K A"
		
		puts $format.option('A', 'Pretvarjanje')
		puts
		puts $format.option('X', 'Izhod')
		puts
		
		$prompt.keypress
	end
end

class Pretvarjanje

	def initialize
		@units = [ "g", "m", "l" ]

		@measures = [
			Measure.new("M", "1000000.0"),
			Measure.new("k", "1000.0"),
			Measure.new("h", "100.0"),
			Measure.new("da", "10.0"),
			Measure.new("", "1.0"),
			Measure.new("d", "0.1"),
			Measure.new("c", "0.01"),
			Measure.new("m", "0.001"),
			Measure.new("µ", "0.000001")
		]
	end

	def run
		loop do
			$format.cls

			$format.put_title "P R E T V A R J A N J E"
			put_measures

			prepare_next
			value_str = $pastel.bold("#{@value.to_string}")
			from_unit_str = "#{@from.unit}#{@unit}"
			to_unit_str = "#{@to.unit}#{@unit}"

			standard = @from.to_standard(@value.to_s())
			result = @to.from_standard(standard)

			puts "  #{value_str} #{from_unit_str} -> #{to_unit_str}"

			count = 0

			loop do
				answer = $prompt.ask("  Odgovor (Enter za izhod):")
				return if answer.nil?

				value = BigDecimal(answer)

				if value != result
					count += 1
					if count == 5
						puts
						puts $pastel.red("  Ni ti uspelo, rešitev je: #{result.to_string} #{to_unit_str}")
						$prompt.keypress
						break
					end

					puts $pastel.red("  Napačno!")
					next
				end

				puts $pastel.green("  PRAVILNO!!!!")
				$prompt.keypress
				break
			end
		end
	end

	private

	def prepare_next
		@value = BigDecimal((rand(1000) + 1).to_s)
		@unit = @units[rand(@units.length())]
		
		@from = @measures[rand(@measures.length())]

		loop do
			@to = @measures[rand(@measures.length())]
			if @to.unit != @from.unit
				break
			end
		end
	end

	def put_measures
		@measures.each do |m|
			next if m.unit.empty?
	
			unit = m.unit.ljust(3)
			value = m.value.to_s("F")
	
			puts $pastel.dim("  #{unit} #{value}")
		end

		puts
	end

	class Measure
		def initialize(u, v)
			@unit = u
			@value = BigDecimal(v)
		end

		def unit
			@unit
		end

		def value
			@value
		end

		def is_standard
			@unit.empty?
		end

		def to_standard(v)
			BigDecimal(v) * @value
		end

		def from_standard(v)
			BigDecimal(v) / @value
		end
	end
end

$prompt = TTY::Prompt.new
$pastel = Pastel.new	
$format = Formatting.new
program = Program.new

loop do
	case program.run
		when 'a'
			Pretvarjanje.new.run
		when 'x'
			break
	end
end