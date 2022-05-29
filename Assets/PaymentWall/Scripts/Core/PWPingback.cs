using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Paymentwall {

    public class PWPingback : PWBase {

        /**
         * Pingback types
         */
        const int PINGBACK_TYPE_REGULAR = 0;
        const int PINGBACK_TYPE_GOODWILL = 1;
        const int PINGBACK_TYPE_NEGATIVE = 2;

        const int PINGBACK_TYPE_RISK_UNDER_REVIEW = 200;
        const int PINGBACK_TYPE_RISK_REVIEWED_ACCEPTED = 201;
        const int PINGBACK_TYPE_RISK_REVIEWED_DECLINED = 202;

        const int PINGBACK_TYPE_SUBSCRIPTION_CANCELLATION = 12;
        const int PINGBACK_TYPE_SUBSCRIPTION_EXPIRED = 13;
        const int PINGBACK_TYPE_SUBSCRIPTION_PAYMENT_FAILED = 14;

		List<string> ipWhitelist = new List<string>() { 
			"174.36.92.186",
			"174.36.96.66",
			"174.36.92.187",
			"174.36.92.192",
			"174.37.14.28" 
		};


        /**
         * Pingback parameters
         */
        protected Dictionary<string, string> parameters = new Dictionary<string, string>();


        /**
         * IP address
         */
        protected string ipAddress;


        /**
         * @param Dictionary<string, string> parameters associative array of parameters received by pingback processing script, e.g. Request.QueryString()
         * @param string ipAddress IP address from where the pingback request orginates, e.g. '127.0.0.1'
         */
        public void Pingback(NameValueCollection parameters, string ipAddress) {
            foreach (string p in parameters.AllKeys) {
                this.parameters.Add(p, parameters[p]);
            }
            this.ipAddress = ipAddress;
        }


        /**
         * Check whether pingback is valid
         * 
         * @param bool skipIpWhiteListCheck if IP whitelist check should be skipped, e.g. if you have a load-balancer changing the IP
         * @return bool
         */
        public bool Validate(bool skipIpWhiteListCheck = false) {
            bool validated = false;

            if (this.IsParametersValid()) {
                if (this.IsIpAddressValid() || skipIpWhiteListCheck) {
                    if (this.IsSignatureValid()) {
                        validated = true;
                    } else {
                        this.appendToErrors("Wrong signature");
                    }
                } else {
                    this.appendToErrors("IP address is not whitelisted");
                }
            } else {
                this.appendToErrors("Missing parameters");
            }

            return validated;
        }


        /**
         * @return bool
         */
        public bool IsSignatureValid()
        {
            string signature = "";
            Dictionary<string, string> signatureParamsToSign = new Dictionary<string, string>();
            if (this.parameters.ContainsKey("sig")) {
                signature = this.parameters["sig"];
            } else {
                signature = null;
            }

            List<string> signatureParams = new List<string>();

            if (PWPingback.GetApiType() == PWPingback.API_VC) {
                signatureParams.AddRange(new string[] { "uid", "currency", "type", "ref" });
            } else if (PWPingback.GetApiType() == PWPingback.API_GOODS) {
                signatureParams.AddRange(new string[] { "uid", "goodsid", "slength", "speriod", "type", "ref" });
            } else { //API_CART
                signatureParams.AddRange(new string[] { "uid", "goodsid", "type", "ref" });
                this.parameters["sign_version"] = PWPingback.SIGNATURE_VERSION_2.ToString();
            }

            if (!this.parameters.ContainsKey("sign_version")) { //Check if signature version 1            
                foreach (string field in signatureParams) {
                    if (this.parameters[field] != null) {
                        signatureParamsToSign.Add(field, this.parameters[field]);
					} else {
                        signatureParamsToSign.Add(field, null);
					}
                }
                this.parameters["sign_version"] = PWPingback.SIGNATURE_VERSION_1.ToString();
            } else {
                signatureParamsToSign = this.parameters;
            }

            string signatureCalculated = this.CalculateSignature(signatureParamsToSign, PWPingback.GetSecretKey(), Convert.ToInt32(this.parameters["sign_version"]));

            return signatureCalculated == signature;
        }


        /**
         * @return bool
         */
        public bool IsIpAddressValid() {
            return ipWhitelist.Contains(this.ipAddress);
        }


        /**
         * @return bool 
         */
        public bool IsParametersValid() {
            int errorsNumber = 0;

			foreach (string field in GetRequiredParams()) {
                if (!this.parameters.ContainsKey(field) || this.parameters[field] == null || this.parameters[field].Equals(" ")) {
                    this.appendToErrors("Parameter " + field + " is missing");
                    errorsNumber++;
                }
            }

            return errorsNumber == 0;
        }

		/*
		 * Get required parameters based on API
		 * 
		 * @return List<string>
		 */
		public List<string> GetRequiredParams() {
			List<string> requiredParams = new List<string>();
			
			if (PWPingback.GetApiType() == PWPingback.API_VC) {
				requiredParams.AddRange(new string[] { "uid", "currency", "type", "ref", "sig" });
			} else if (PWPingback.GetApiType() == PWPingback.API_GOODS) {
				requiredParams.AddRange(new string[] { "uid", "goodsid", "type", "ref", "sig" });
			} else { //API_CART
				requiredParams.AddRange(new string[] { "uid", "goodsid[0]", "type", "ref", "sig" });
			}

			return requiredParams;
		}


        /**
         * Get pingback parameter
         * 
         * @param string param
         * @return string
         */
        public string GetParameter(string param) {
            if (this.parameters[param]!=null) {
                return this.parameters[param];
			} else {
                return null;
			}
        }


        /**
         * Get pingback parameter "type"
         * 
         * @return int
         */
        public int GetPingbackType() {   //changed to getPingbackType() to avoid duplicate name with C# method getType()
            if (this.parameters["type"] != null) {
                return Convert.ToInt32(this.parameters["type"]);
			} else {
                return -1;
			}
        }


        /**
         * Get verbal explanation of the informational pingback
         * 
         * @return string
         */
        public string GetTypeVerbal() {
            Dictionary<string, string> pingbackTypes = new Dictionary<string, string>();
            pingbackTypes.Add(PWPingback.PINGBACK_TYPE_SUBSCRIPTION_CANCELLATION.ToString(), "user_subscription_cancellation");
            pingbackTypes.Add(PWPingback.PINGBACK_TYPE_SUBSCRIPTION_EXPIRED.ToString(), "user_subscription_expired");
            pingbackTypes.Add(PWPingback.PINGBACK_TYPE_SUBSCRIPTION_PAYMENT_FAILED.ToString(), "user_subscription_payment_failed");

            if (!(this.parameters["type"]==null) || this.parameters["type"].Equals(" ")) {
                if (pingbackTypes.ContainsKey(this.parameters["type"])) {
                    return pingbackTypes[this.parameters["type"]];
				} else {
                    return null;
				}
            } else {
                return null;
            }
        }


        /**
         * Get pingback parameter "uid"
         * 
         * @return string
         */
        public string GetUserId() {
            return this.GetParameter("uid");
        }


        /**
         * Get pingback parameter "currency"
         * 
         * @return string
         */
        public string GetVirtualCurrencyAmount() {
            return this.GetParameter("currency");
        }


        /**
         * Get product id
         * 
         * @return string
         */
        public string GetProductId() {
            return this.GetParameter("goodsid");
        }


        /**
         * @return int
         */
        public int GetProductPeriodLength() {
            return Convert.ToInt32(this.GetParameter("slength"));
        }


        /*
         * @return string
         */
        public string GetProductPeriodType() {
            return this.GetParameter("speriod");
        }

        /*
         * @return List<Paymentwall_Product>
         */
        public List<PWProduct> GetProducts() {
            List<PWProduct> products = new List<PWProduct>();
            List<string> productIds = new List<string>();

            foreach (var productId in this.parameters["goodsid"]) {
                productIds.Add(productId.ToString());
            }

            if (productIds.Any()) {
                foreach (string id in productIds) {
                    products.Add(new PWProduct(id));
                }
            }

            return products;
        }


        /*
         * Get pingback parameter "ref"
         * 
         * @return string
         */
        public string GetReferenceId() {
            return this.GetParameter("ref");
        }


        /*
         * Returns unique identifier of the pingback that can be used for checking
         * If the same pingback was already processed by your servers
         * Two pingbacks with the same unique ID should not be processed more than once
         * 
         * @return string
         */
        public string GetPingbackUniqueId() {
            return this.GetReferenceId() + "_" + this.GetPingbackType();
        }


        /*
         * Check wheter product is deliverable
         * 
         * @return bool
         */
        public bool IsDeliverable() {
            return (
              this.GetPingbackType() == PWPingback.PINGBACK_TYPE_REGULAR ||
              this.GetPingbackType() == PWPingback.PINGBACK_TYPE_GOODWILL ||
              this.GetPingbackType() == PWPingback.PINGBACK_TYPE_RISK_REVIEWED_ACCEPTED
            );
        }


        /*
         * Check wheter product is cancelable
         * 
         * @return bool
         */
        public bool IsCancelable() {
            return (
                this.GetPingbackType() == PWPingback.PINGBACK_TYPE_NEGATIVE ||
                this.GetPingbackType() == PWPingback.PINGBACK_TYPE_RISK_REVIEWED_DECLINED
            );
        }


        /*
         * Check whether product is under review
         * 
         * @return bool
         */
        public bool IsUnderReview() {
            return this.GetPingbackType() == PWPingback.PINGBACK_TYPE_RISK_UNDER_REVIEW;
        }


        /*
         * Build signature for the pingback received
         * 
         * @param Dictionary<string, string> parameters
         * @param string secret Paymentwall Secret Key
         * @param int version Paymentwall Signature Version
         * @return string
         */
        public string CalculateSignature(Dictionary<string, string> signatureParamsToSign, string secret, int version) {
            string baseString = "";
            signatureParamsToSign.Remove("sig");

            if (version == PWPingback.SIGNATURE_VERSION_2 || version == PWPingback.SIGNATURE_VERSION_3) {
                signatureParamsToSign = signatureParamsToSign.OrderBy(d => d.Key, StringComparer.Ordinal).ToDictionary(d => d.Key, d => d.Value);
            }

            foreach (KeyValuePair<string, string> kvp in signatureParamsToSign) {
                baseString += kvp.Key + "=" + kvp.Value;
            }
            baseString += secret;

            if (version == PWPingback.SIGNATURE_VERSION_3) {
                return PWPingback.getHash(baseString, "sha256");
            } else {
                return PWPingback.getHash(baseString, "md5");
            }
        }

    }
}