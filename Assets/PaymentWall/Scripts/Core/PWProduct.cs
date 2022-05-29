using System;
using System.Collections.Generic;
using System.Linq;

namespace Paymentwall {

    public class PWProduct {

        /**
         * Product types 
         */
        public const string TYPE_SUBSCRIPTION = "subscription";
        public const string TYPE_FIXED = "fixed";


        /**
         * Product period types
         */
        public const string PERIOD_TYPE_DAY = "day";
        public const string PERIOD_TYPE_WEEK = "week";
        public const string PERIOD_TYPE_MONTH = "month";
        public const string PERIOD_TYPE_YEAR = "year";


        /**
         * Paymentwall_Product class's properties
         */
        private string productId;
        private float amount;
        private string currencyCode;
        private string name;
        private string productType;
        private int periodLength;
        private string periodType;
        private bool recurring;
        private PWProduct trialProduct;


        /**
         * Default constructor
         */
        public PWProduct() { }


        /**
         * @param string productId your internal product ID, e.g. product1
         * @param float amount product price, e.g. 9.99
         * @param string currencyCode ISO currency code, e.g. USD
         * @param string name product name
         * @param string productType product type, Paymentwall_Product::TYPE_SUBSCRIPTION for recurring billing, or Paymentwall_Product::TYPE_FIXED for one-time payments
         * @param int periodLength product period length, e.g. 3. Only required if product type is subscription
         * @param string periodType product period type, e.g. Paymentwall_Product::PERIOD_TYPE_MONTH. Only required if product type is subscription
         * @param bool recurring if the product recurring
         * @param Paymentwall_Product trialProduct trial product, product type should be subscription, recurring should be True
         */
        public PWProduct(string productId, float amount = 0.0f, 
		                 string currencyCode = null, string name = null, 
		                 string productType = PWProduct.TYPE_FIXED, 
		                 int periodLength = 0, string periodType = null, 
		                 bool recurring = false, PWProduct trialProduct = null) {
            this.productId = productId;
            this.amount = amount;
            this.currencyCode = currencyCode;
            this.name = name;
            this.productType = productType;
            this.periodLength = periodLength;
            this.periodType = periodType;
            this.recurring = recurring;

            if (productType == PWProduct.TYPE_SUBSCRIPTION && this.recurring == true) {
                this.trialProduct = trialProduct;
            } else {
                this.trialProduct = null;
            }
        }


        /**
         * @return string product ID
         */
        public string GetId() {
            return this.productId;
        }


        /**
         * @return float product price, e.g. 96.69
         */
        public float GetAmount() {
            return this.amount;
        }

        /**
	     * @return string ISO currency code, e.g. USD
	     */
        public string GetCurrencyCode() {
            return this.currencyCode;
        }

        /**
	     * @return string product name
	     */
        public string GetName() {
            return this.name;
        }

        /**
	     * @return string product type, Paymentwall_Product::TYPE_SUBSCRIPTION for recurring billing, Paymentwall_Product::TYPE_FIXED for one-time
	     */
        public string GetProductType() {
            return this.productType;
        }

        /**
	     * @return string product period type, e.g. Paymentwall_Product::PERIOD_TYPE_MONTH
	     */
        public string GetPeriodType() {
            return this.periodType;
        }

        /**
	     * @return string product period length, e.g. 3
	     */
        public int GetPeriodLength() {
            return this.periodLength;
        }

        /**
         * @return bool if the product recurring
         */
        public bool IsRecurring() {
            return this.recurring;
        }

        /**
         * @return Paymentwall_Product trial product
         */
        public PWProduct GetTrialProduct() {
            return this.trialProduct;
        }

    }
}