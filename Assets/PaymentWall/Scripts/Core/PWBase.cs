using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Paymentwall {

	public enum API_MODE {
		TEST,
		LIVE
	}

	public abstract class PWBase {

		/*
         * Paymentwall library version
         */
		const string LIB_VERSION = "1.0.0";

		/*
         * URLs for Paymentwall Token
         */
		public const string TOKEN_URL = "https://api.paymentwall.com/api/brick/token";

		/*
         * URLs for Paymentwall Pro
         */
		private const string CHARGE_URL = "https://api.paymentwall.com/api/pro/v1/charge";
		private const string SUBS_URL = "https://api.paymentwall.com/api/pro/v1/subscription";
		private const string CANCEL_SUBS_URL = "https://api.paymentwall.com/api/brick/subscritpion/";

		/*
		 * URLs for Testing
		 */
		private const string TEST_CHARGE_URL = "https://api.paymentwall.com/api/brick/charge";
		private const string TEST_SUBS_URL = "https://api.paymentwall.com/api/brick/subscription";

		/*
         * API types
         */
		public const int API_VC = 1;
		public const int API_GOODS = 2;
		public const int API_CART = 3;

		/*
         * Controllers for APIs
         */
		protected const string CONTROLLER_PAYMENT_VIRTUAL_CURRENCY = "ps";
		protected const string CONTROLELR_PAYMENT_DIGITAL_GOODS = "subscription";
		protected const string CONTROLLER_PAYMENT_CART = "cart";

		/**
	     * Signature versions
	     */
		protected const int DEFAULT_SIGNATURE_VERSION = 3;
		protected const int SIGNATURE_VERSION_1 = 1;
		protected const int SIGNATURE_VERSION_2 = 2;
		protected const int SIGNATURE_VERSION_3 = 3;
		protected List<string> errors = new List<string> ();

		/**
         * Paymentwall API type
         * @param int apiType
         */
		public static int apiType;

		/**
         * Paymentwall application key - can be found in your merchant area
         * @param string appKey
         */
		public static string appKey;

		/**
         * Paymentwall secret key - can be found in your merchant area
         * @param string secretKey
         */
		public static string secretKey;

		/**
         * Paymentwall Pro API Key
         * @param string proApiKey
         */
		public static string proApiKey;

		/*
		 * Paymentwall API Mode
		 * @param API_MODE apiMode
		 */
		public static API_MODE apiMode = API_MODE.TEST;

		public static void SetApiMode (API_MODE mode) {
			PWBase.apiMode = mode;
		}

		public static API_MODE GetApiMode () {
			return PWBase.apiMode;
		}

		public static string GetChargeURL () {
			if (apiMode == API_MODE.TEST) {
				return PWBase.TEST_CHARGE_URL;
			} else {
				return PWBase.CHARGE_URL;
			}
		}

		public static string GetSubscriptionURL () {
			if (apiMode == API_MODE.TEST){
				return PWBase.TEST_SUBS_URL;
			} else {
				return PWBase.SUBS_URL;
			}
		}

		public static string GetCancelSubscriptionURL() {
			return PWBase.CANCEL_SUBS_URL;
		}

		/*
         * @param int apiType API type
         */
		public static void SetApiType (int apiType) {
			PWBase.apiType = apiType;
		}

		public static int GetApiType () {
			return PWBase.apiType;
		}

		/*
         * @param string appKey application key of your application, can be found inside of your Paymentwall Merchant Account
         */
		public static void SetAppKey (string appKey) {
			PWBase.appKey = appKey;
		}

		public static string GetAppKey () {
			return PWBase.appKey;
		}

		/*
         *  @param string secretKey secret key of your application, can be found inside of your Paymentwall Merchant Account
         */
		public static void SetSecretKey (string secretKey) {
			PWBase.secretKey = secretKey;
		}

		public static string GetSecretKey () {
			return PWBase.secretKey;
		}

		/*
         * @param string proApiKey API key used for Pro authentication
         */
		public static void SetProApiKey (string proApiKey) {
			PWBase.proApiKey = proApiKey;
		}

		public static string GetProApiKey () {
			return PWBase.proApiKey;
		}

		/*
         * Fill the array with the errors found at execution
         * 
         * @param string err
         * @return int
         */
		protected void appendToErrors (string err) {
			this.errors.Add (err);
		}

		/**
         * Return errors
         * 
         * @return List<string>
         */
		public List<string> getErrors () {
			return this.errors;
		}

		/*
         * Return error summary
         * 
         * @return string
         */
		public string getErrorSummary () {
			return string.Join ("\n", this.getErrors ().ToArray ());
		}

		
		
		/*
         * Generate a hased string
         * 
         * @param string inputString The string to be hased
         * @param string algorithm The hash algorithm, e.g. md5, sha256
         * @return string hashed string
         */
		protected static string getHash (string inputString, string algorithm) {
			HashAlgorithm alg = null;
			
			if (algorithm == "md5"){
				alg = MD5.Create ();
			} else if (algorithm == "sha256") {
				alg = SHA256.Create ();
			}
			
			byte[] hash = alg.ComputeHash (Encoding.UTF8.GetBytes (inputString));
			
			StringBuilder sb = new StringBuilder ();
			for (int i = 0; i < hash.Length; i++) {
				sb.Append (hash [i].ToString ("X2"));
			}
			return sb.ToString ().ToLower ();
		}

	}
}