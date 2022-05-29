using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Paymentwall
{
    public class PWWidget : PWBase
    {

        /**
	     * Widget call URL
	     */
        const string BASE_URL = "https://api.paymentwall.com/api";


        protected string userId;
        protected string widgetCode;
        protected List<PWProduct> products = new List<PWProduct>();
        protected Dictionary<string, string> extraParams = new Dictionary<string, string>();


        /*
         * @param string userId identifier of the end-user who is viewing the widget
         * @param string widgetCode e.g. p1 or p1_1, can be found inside of your Paymentwall Merchant account in the Widgets section
         * @param List<Paymentwall_Product> products array that consists of Paymentwall_Product entities; for Flexible Widget Call use array of 1 product
         * @param Dictionary<string, string> extraParams associative array of additional params that will be included into the widget URL, 
         * e.g. 'sign_version' or 'email'. Full list of parameters for each API is available at http://paymentwall.com/documentation
         */
        public PWWidget(string userId, string widgetCode, List<PWProduct> products, Dictionary<string, string> extraParams) {
            this.userId = userId;
            this.widgetCode = widgetCode;
            this.products = products;
            this.extraParams = extraParams;
        }


        /*
         * Widget constructor for Virtual Currency API
         * 
         * @param string userId identifier of the end-user who is viewing the widget
         * @param string widgetCode e.g. p1 or p1_1, can be found inside of your Paymentwall Merchant account in the Widgets section
         * @param Dictionary<string, string> extraParams associative array of additional params that will be included into the widget URL, 
         * e.g. 'sign_version' or 'email'. Full list of parameters for each API is available at http://paymentwall.com/documentation
         */
        public PWWidget(string userId, string widgetCode, Dictionary<string, string> extraParams) {
            this.userId = userId;
            this.widgetCode = widgetCode;
            this.extraParams = extraParams;
            this.products = new List<PWProduct>();
        }


        /*
         * Get default signature version for this API Type
         * 
         * @return int
         */
        public int GetDefaultSignatureVersion() {
            if (PWWidget.GetApiType() != PWWidget.API_CART) {
                return PWWidget.DEFAULT_SIGNATURE_VERSION;
            } else {
                return PWWidget.SIGNATURE_VERSION_2;
            }
        }


        /*
         * Return URL for the widget
         * 
         * @return string
         */
        public string GetUrl() {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["key"] = PWWidget.GetAppKey();
            parameters["uid"] = this.userId;
            parameters["widget"] = this.widgetCode;

            int productsNumber = this.products.Count;

            if (PWWidget.GetApiType() == PWWidget.API_GOODS) {
                if (productsNumber > 0) {
                    if (productsNumber == 1) {
                        PWProduct product = this.products[0];
                        PWProduct postTrialProduct = null;
                        if (product.GetTrialProduct() is PWProduct) {
                            postTrialProduct = product;
                            product = product.GetTrialProduct();
                        }
                        parameters.Add("amount", product.GetAmount().ToString());
                        parameters.Add("currencyCode", product.GetCurrencyCode());
                        parameters.Add("ag_name", product.GetName());
                        parameters.Add("ag_external_id", product.GetId());
                        parameters.Add("ag_type", product.GetProductType());

                        if (product.GetProductType() == PWProduct.TYPE_SUBSCRIPTION) {
                            parameters.Add("ag_period_length", product.GetPeriodLength().ToString());
                            parameters.Add("ag_period_type", product.GetPeriodType());

                            if (product.IsRecurring()) {
                                parameters.Add("ag_recurring", (Convert.ToInt32(product.IsRecurring())).ToString());

                                if (postTrialProduct != null) {
                                    parameters.Add("ag_trial", "1");
                                    parameters.Add("ag_post_trial_external_id", postTrialProduct.GetId());
                                    parameters.Add("ag_post_trial_period_length", postTrialProduct.GetPeriodLength().ToString());
                                    parameters.Add("ag_post_trial_period_type", postTrialProduct.GetPeriodType());
                                    parameters.Add("ag_post_trial_name", postTrialProduct.GetName());
                                    parameters.Add("post_trial_amount", postTrialProduct.GetAmount().ToString());
                                    parameters.Add("post_trial_currencyCode", postTrialProduct.GetCurrencyCode().ToString());
                                }

                            }

                        }

                    } //end if (productNumber == 1)
					else {
                        //TODO: Paymentwall_Widget.appendToErrors('Only 1 product is allowed in flexible widget call');
                    }

                } //end if (productNumber > 0) 

            } else if (PWWidget.GetApiType() == PWWidget.API_CART) {
                int index = 0;

                foreach (PWProduct product in this.products) {
                    parameters.Add("external_ids[" + index.ToString() + "]", product.GetId());

                    if (product.GetAmount() != -1f) {
                        parameters.Add("prices[" + index.ToString() + "]", product.GetAmount().ToString());
                    }

                    if (product.GetCurrencyCode() != null) {
                        parameters.Add("currencies[" + index.ToString() + "]", product.GetCurrencyCode());
                    }

                    index++;
                }

                index = 0;
            }

            int signatureVersion = this.GetDefaultSignatureVersion();
            parameters.Add("sign_version", Convert.ToString(signatureVersion));

            if (this.extraParams.ContainsKey("sign_version")) {
                parameters["sign_version"] = this.extraParams["sign_version"];
                signatureVersion = Convert.ToInt32(this.extraParams["sign_version"]);
            }
            parameters = MergeDictionaries(parameters, extraParams);

            parameters["sign"] = PWWidget.CalculateSignature(parameters, PWWidget.GetSecretKey(), signatureVersion);

            return PWWidget.BASE_URL + "/" + this.BuildController(this.widgetCode) + "?" + this.BuildQueryString(parameters, "&");
        }


        /**
	     * Return HTML code for the widget
	     *
	     * @param Dictionary<string, string> attributes associative array of additional HTML attributes, e.g. Dictionary.Add("width", "100%")
	     * @return string
	     */
//        public string getHtmlCode(Dictionary<string, string> attributes = null)
//        {
//            Dictionary<string, string> defaultAttributes = new Dictionary<string, string>();
//            defaultAttributes.Add("frameborder", "0");
//            defaultAttributes.Add("width", "750");
//            defaultAttributes.Add("height", "800");
//            if (attributes != null)
//            {
//                attributes = mergeDictionaries(defaultAttributes, attributes);
//            }
//            else
//            {
//                attributes = defaultAttributes;
//            }
//            var attributesQuery = this.buildQueryString(attributes, " ");
//            return "<iframe src='" + this.getUrl() + "' " + attributesQuery + "></iframe>";
//        }


        /**
         * Build controller URL depending on API type
         *
         * @param string widget code of the widget
         * @param bool flexibleCall
         * @return string
         */
        protected string BuildController(string widget, bool flexibleCall = false) {
            if (PWWidget.GetApiType() == PWWidget.API_VC) {
                if (!Regex.IsMatch(widget, @"^w|s|mw")) {
                    return PWWidget.CONTROLLER_PAYMENT_VIRTUAL_CURRENCY;
                } else {
                    return "";
                }
            } else if (PWWidget.GetApiType() == PWWidget.API_GOODS) {
                if (!flexibleCall) {
                    if (!Regex.IsMatch(widget, @"^w|s|mw")) {
                        return PWWidget.CONTROLELR_PAYMENT_DIGITAL_GOODS;
                    } else {
                        return "";
                    }
                } else {
                    return PWWidget.CONTROLELR_PAYMENT_DIGITAL_GOODS;
                }
            } else {
                return PWWidget.CONTROLLER_PAYMENT_CART;
            }
        }


        /**
	     * Build signature for the widget specified
	     *
	     * @param Dictionary<string, string> parameters
	     * @param string secret Paymentwall Secret Key
	     * @param int version Paymentwall Signature Version
	     * @return string
	     */
        public static string CalculateSignature(Dictionary<string, string> parameters, string secret, int version) {
            string baseString = "";

            if (version == PWWidget.SIGNATURE_VERSION_1) {   //TODO: throw exception if no uid parameter is present 
                if (parameters["uid"] != null) {
                    baseString += parameters["uid"];
				} else {
                    baseString += secret;
				}
                return PWWidget.getHash(baseString, "md5");
            } else {
				parameters = parameters.OrderBy(d => d.Key, StringComparer.Ordinal).ToDictionary(d => d.Key, d => d.Value);

                foreach (KeyValuePair<string, string> param in parameters) {
                    baseString += param.Key + "=" + param.Value;
                }
                baseString += secret;

                if (version == PWWidget.SIGNATURE_VERSION_2) {
                    return PWWidget.getHash(baseString, "md5");
				} else {
                    return PWWidget.getHash(baseString, "sha256");
				}
            }
        }


        /*
         * Build the query string
         * 
         * @param Dictionary<string, string> dict The input dictionary
         * @param string s The connector sign, e.g. &, =, or white space
         * @return string
         */
        private string BuildQueryString(Dictionary<string, string> dict, string s) {
            var queryString = new StringBuilder();

            int count = 0;
            bool end = false;

			foreach(string key in dict.Keys) {
				if(count == dict.Count - 1) {
					end = true;
				}

				string escapedValue = Uri.EscapeDataString(dict[key]??string.Empty);
				if(end) {
					queryString.AppendFormat("{0}={1}",key,escapedValue);
				} else {
					queryString.AppendFormat("{0}={1}{2}",key,escapedValue,s);
				}

				count++;
			}
            return queryString.ToString();
        }


        /**
         * Merging 2 dictionaries to 1 dictionary
         * 
         * @param dict1 Dictionary<string, string> The first dictionary
         * @param dict2 Dictionar<string, string> The second dictionary
         * @return Dictionary<string, string> The merged dictionary
         */
        private Dictionary<string, string> MergeDictionaries(Dictionary<string, string> dict1, Dictionary<string, string> dict2) {
            foreach (KeyValuePair<string, string> kvp in dict2) {
                if (dict1.ContainsKey(kvp.Key)) {
                    dict1[kvp.Key] = kvp.Value;
				} else {
                    dict1.Add(kvp.Key, kvp.Value);
				}
            }
            return dict1;
        }
    }
}