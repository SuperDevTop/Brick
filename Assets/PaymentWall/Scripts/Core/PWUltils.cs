using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

namespace Paymentwall {
	public enum CC_TYPE {
		Invalid,
		Visa,
		MasterCard,
		Discover,
		Amex
	}

	public class PWUltils {

		private const string MatchEmailPattern =
			@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
				+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
				+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
				+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

		private const string cardRegex = "^(?:(?<Visa>4\\d{3})|(?<MasterCard>5[1-5]\\d{2})|(?<Discover>6011)|(?<DinersClub>(?:3[68]\\d{2})|(?:30[0-5]\\d))|(?<Amex>3[47]\\d{2}))([ -]?)(?(DinersClub)(?:\\d{6}\\1\\d{4})|(?(Amex)(?:\\d{6}\\1\\d{5})|(?:\\d{4}\\1\\d{4}\\1\\d{4})))$";

		public static bool IsCreditCardNumberValid (string cardNumber) {
			if (string.IsNullOrEmpty (cardNumber) || !IsPassLuhnTest(cardNumber)) {
				return false;
			} else {
				if(GetCardTypeFromNumber(cardNumber)!= CC_TYPE.Invalid) {
					return true;
				} else {
					return false;
				}
			}
		}

		public static bool IsExpireDateValid(string expDate) {
			if (string.IsNullOrEmpty (expDate)) {
				return false;
			} else {
				string dateTime = expDate.Replace("/",string.Empty);
				int monVal = int.Parse(dateTime.Substring (0, 2));
				int yearVal = int.Parse(dateTime.Substring(2,2));
				if(monVal > 12) {
					return false;
				}
				int curyear = DateTime.Today.Year;
				int curMonth = DateTime.Today.Month;
				yearVal += 2000;
				if(yearVal == curyear && monVal <= curMonth) {
					return false;
				}
				if (yearVal < curyear) {
					return false;
				}
			}
			return true;
		}
		
		public static bool IsCVVNumberValid (string cvv) {

			if (string.IsNullOrEmpty (cvv)) {
				return false;
			} else {
				return true;
			}
		}
		
		public static bool IsEmailValid (string email) {
			if (!Regex.IsMatch (email, MatchEmailPattern)) {
				return false;
			} else {
				return true;
			}
		}
		
		public static CC_TYPE GetCardTypeFromNumber(string cardNum)
		{
			//Create new instance of Regex comparer with our
			//credit card regex patter
			Regex cardTest = new Regex(cardRegex);
			
			//Compare the supplied card number with the regex
			//pattern and get reference regex named groups
			GroupCollection gc = cardTest.Match(cardNum).Groups;
			//Compare each card type to the named groups to 
			//determine which card type the number matches

			if (gc[CC_TYPE.Amex.ToString()].Success) {
				return CC_TYPE.Amex;
			}
			else if (gc[CC_TYPE.MasterCard.ToString()].Success) {
				return CC_TYPE.MasterCard;
			}
			else if (gc[CC_TYPE.Visa.ToString()].Success) {
				return CC_TYPE.Visa;
			}
			else if (gc[CC_TYPE.Discover.ToString()].Success) {
				return CC_TYPE.Discover;
			} else {
				//Card type is not supported by our system, return null
				//(You can modify this code to support more (or less)
				// card types as it pertains to your application)
				Debug.Log ("Not supported in our payment's system");
				return CC_TYPE.Invalid;
			}
		}

		public static bool IsPassLuhnTest(string cardNum) {
			//Clean the card number- remove dashes and spaces
			// cardNum = cardNum.Replace("-", "").Replace(" ", "");
			
			//Convert card number into digits array
			int[] digits = new int[cardNum.Length];
			for (int len = 0; len < cardNum.Length; len++)
			{
				digits[len] = int.Parse(cardNum.Substring(len, 1));
			}
			
			//Luhn Algorithm
			//Adapted from code availabe on Wikipedia at
			//http://en.wikipedia.org/wiki/Luhn_algorithm
			int sum = 0;
			bool alt = false;
			for (int i = digits.Length - 1; i >= 0; i--)
			{
				int curDigit = digits[i];
				if (alt)
				{
					curDigit *= 2;
					if (curDigit > 9)
					{
						curDigit -= 9;
					}
				}
				sum += curDigit;
				alt = !alt;
			}
			
			//If Mod 10 equals 0, the number is good and this will return true
			return sum % 10 == 0;
		}
	}
}