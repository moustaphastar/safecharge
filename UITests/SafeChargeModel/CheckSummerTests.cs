using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UI.SafeChargeModel.Tests
{
    [TestClass()]
    public class CheckSummerTests
    {
        [TestMethod()]
        public void GetChecksumSha256Test()
        {
            var check = new CheckSummer();
            string result1 = check.GetChecksumSha256("ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP613798893296892179520397810EUR20200101131211");

            string result = check.GetChecksumSha256("ClaKwuxG7LnJpUpcJJMdSXRHyYzxskzYNRCZZOiHdpYeyPvMlDirRgxObmLUk8EP61379889329689217952039784.0.0507dc1ee-071310000USDTest Product10110.00RodneyAtlantis Avenue 1cc_cardMackeyVancouveren_US001 123 456 7891000test@test.comCAhttps://sandbox.safecharge.com/lib/demo_process_request/response.phphttps://ppp-test.safecharge.com/ppp/defaultCancel.dohttps://sandbox.safecharge.com/lib/demo_process_request/response.php2020-07-20 05:23:53");

            Assert.IsTrue(result == "51e65457a42c0cdf7a058e6cd4eea19988ca34e14e60f443f2788a29118603d9");
        }
    }
}