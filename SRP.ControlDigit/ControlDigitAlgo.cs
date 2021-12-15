namespace SRP.ControlDigit
{
    public static class Extensions
    {
        public static int CalculateSumOfDigitsInOddNumberedPositionsStartingFromEnd(long number)
        {
            var sum = 0;
            var num = number.ToString();
            for (var i = num.Length - 1; i >= 0; i -= 2)
                sum += int.Parse(num[i].ToString());
            return sum;
        }
		
        public static int CalculateSumOfDigitsInEvenNumberedPositionsStartingFromEnd(long number)
        {
            var sum = 0;
            var num = number.ToString();
            for (var i = num.Length - 2; i >= 0; i -= 2)
                sum += int.Parse(num[i].ToString());
            return sum;
        }
		
        public static int CalculateCheckDigit(int sum)
        {
            var remainder = sum % 10;
            return remainder == 0 ? 0 : 10 - remainder;
        }
    }
	
	public class ControlDigitAlgo
	{
        //cDA - controlDigitAlgo
        public static int Upc(long number)
		{
            var oddSum = Extensions.CalculateSumOfDigitsInOddNumberedPositionsStartingFromEnd(number);
            var evenSum = Extensions.CalculateSumOfDigitsInEvenNumberedPositionsStartingFromEnd(number);
            return Extensions.CalculateCheckDigit(oddSum * 3 + evenSum);
        }
		
		public static char Isbn10(long number)
		{
			if (number == 0)
				return '0';
            var sum = 0;
            var num = number.ToString();
            for (var i = num.Length - 1; i >= 0; i--)
                sum += int.Parse(num[i].ToString()) * (num.Length - i + 1);
            var theCheckDigit = 11 - sum % 11;
            return theCheckDigit == 10 ? 'X' : theCheckDigit.ToString()[0];
        }
		
		public static int Isbn13(long number)
		{
            var oddSum = Extensions.CalculateSumOfDigitsInOddNumberedPositionsStartingFromEnd(number);
            var evenSum = Extensions.CalculateSumOfDigitsInEvenNumberedPositionsStartingFromEnd(number);
            return Extensions.CalculateCheckDigit(oddSum + evenSum * 3);
        }
	}
}