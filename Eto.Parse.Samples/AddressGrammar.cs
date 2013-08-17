using System;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples
{
	public class AddressGrammar : Grammar
	{
		public AddressGrammar()
			: base("postal-address")
		{
			var terminal = +Terminals.LetterOrDigit;

			var aptNum = (((Parser)"Apt" | "Suite") & "#" & +Terminals.Digit).Named("apt-num");
			var streetType = ((Parser)"Street" | "Drive" | "Ave" | "Avenue").Named("street-type");
			var street = (terminal.Named("street-name") & ~streetType).Named("street");
			var zipPart = (terminal.Named("town-name") & "," & terminal.Named("state-code") & terminal.Named("zip-code")).Named("zip");
			var streetAddress = (terminal.Named("house-num") & street & aptNum).Named("street-address");

			// name
			var suffixPart = ((Parser)"Sr." | "Jr." | +Terminals.Set("IVXLCDM")).Named("suffix");
			var personalPart = (terminal.Named("first-name") | (Terminals.Letter & ".")).Named("personal");
			var namePart = new UnaryParser("name");
			namePart.Inner = (personalPart & terminal.Named("last-name") & ~suffixPart) | (personalPart & namePart); // recursion

			this.Inner = namePart & Terminals.Eol & streetAddress & ~Terminals.Eol & zipPart;
		}
	}
}

