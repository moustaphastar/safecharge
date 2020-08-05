using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using UI.Helpers;

namespace UI.SafeChargeModel.Tests
{
    [TestClass()]
    public class CheckSummerTests
    {
        private const string SECRET = "ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP";
        private const string MERCHANT_ID = "6137988932968921795";
        private const string MERCHANT_SITE_ID = "203978";
        private const string CURRENCY = "EUR";

        [TestMethod()]
        public void GetChecksumSha256Test()
        {
            var provider = new CheckSumProvider();
            string result = provider.GetChecksumSha256("ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP613798893296892179520397810EUR20200101131211");

            string result1 = provider.GetChecksumSha256("ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP61379889329689217952039784.0.0507dc1ee-071310000USDTest Product10110.00RodneyAtlantis Avenue 1cc_cardMackeyVancouveren_US001 123 456 7891000test@test.comCAhttps://sandbox.safecharge.com/lib/demo_process_request/response.phphttps://ppp-test.safecharge.com/ppp/defaultCancel.dohttps://sandbox.safecharge.com/lib/demo_process_request/response.php2020-07-20 05:23:53");

            Assert.IsTrue(result1 == "51e65457a42c0cdf7a058e6cd4eea19988ca34e14e60f443f2788a29118603d9");
        }

        [TestMethod()]
        public void CheckoutPageModelChecksumSha256Test()
        {
            var provider = new CheckSumProvider();
            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string stamp = "2018-05-15.02:35:21";
            string itemList1 = "Test+Product50.001";
            string itemList2 = "Test Product50.001";

            List<string> texts = new List<string>
            {
                string.Concat(SECRET, MERCHANT_ID, CURRENCY, "50.00", timeStamp),
                string.Concat(SECRET, MERCHANT_ID, CURRENCY, "50.00", stamp),
                string.Concat(SECRET, MERCHANT_ID, CURRENCY, "50.00", itemList1, timeStamp),
                string.Concat(SECRET, MERCHANT_ID, CURRENCY, "50.00", itemList1, stamp),
                string.Concat(SECRET, MERCHANT_ID, CURRENCY, "50.00", itemList2, timeStamp),
                string.Concat(SECRET, MERCHANT_ID, CURRENCY, "50.00", itemList2, stamp),
                string.Concat(SECRET, MERCHANT_ID, "50.00", CURRENCY, timeStamp),
                string.Concat(SECRET, MERCHANT_ID, "50.00", CURRENCY, stamp),
                string.Concat(SECRET, MERCHANT_ID, "50.00", CURRENCY, itemList1, timeStamp),
                string.Concat(SECRET, MERCHANT_ID, CURRENCY, "50.00", itemList1, stamp),
                string.Concat(SECRET, MERCHANT_ID, "50.00", CURRENCY, itemList2, timeStamp),
                string.Concat(SECRET, MERCHANT_ID, CURRENCY, "50.00", itemList2, stamp)
            };

            List<string> checksums = new List<string>();

            texts.ForEach(x => checksums.Add(provider.GetChecksumSha256(x)));


            Assert.IsTrue(checksums.Contains("66ce9f4ce1e5f47298e7e5e457d0b21ca8d6a668d549240929924054db6d1a21"));
        }
    }
}