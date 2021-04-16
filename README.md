![Build Status](https://github.com/moustaphastar/safecharge/actions/workflows/dotnet.yml/badge.svg)

This is a project for testing nuvei's (former safecharge) payment gateway api and implemented during a test cycle on uTest. The api might have been changed since then, so please refer to nuvei's [official documentation](https://docs.safecharge.com/documentation/payment-overview/intro/) and [api reference](https://www.safecharge.com/docs/API/main/indexMain_v1_0.html?json#).

## Safecharge Integrations
Integration examples 
Repository Branch | Implemented Safecharge Solution
------------ | -------------
[checkoutpage](https://github.com/moustaphastar/safecharge/tree/checkoutpage) | [Checkout page](https://docs.safecharge.com/documentation/accept-payment/checkout-page/quick-start/)
[websdk](https://github.com/moustaphastar/safecharge/tree/websdk) | [Web SDK](https://docs.safecharge.com/documentation/accept-payment/web-sdk/quick-start/)
[servertoserver](https://github.com/moustaphastar/safecharge/tree/servertoserver) | [Server-to-server](https://docs.safecharge.com/documentation/accept-payment/server-to-server/)

Safecharge offers 3 integration solutions:
1) **Checkout page** : Processes payments using safecherge's own checkout page. 
2) **Web SDK** : Starting a payment order session at safecharge via backend request then completes payment process using safecharge's web sdk at client side. For official documentation refer to : https://docs.safecharge.com/documentation/accept-payment/web-sdk/quick-start/
3) **Server to server** : An example using server to server flow.

[Comparison of solutions](https://docs.safecharge.com/documentation/payment-overview/intro/#nuvei-integration-solutions-compared)

## Requirements
- Net Framework 4.7.2. [Download link](https://dotnet.microsoft.com/download).
- Sandbox account for payment integration and credentials. [Safecharge's website](https://www.safecharge.com/).

## Installing & running
1. Download the repository or clone by `git clone https://github.com/moustaphastar/safecharge.git`.
2. Restore the nuget packages using `dotnet restore` from a command line at the root folder.
3. Update web.config file with the credentials from sandbox account:
```xml
<!-- Application wide Safecharge credentials and api endpoints -->
  <add key="safeChargeMerchantSecretKey" value="MERCHANT_SECRET_KEY_FROM_SANDBOX_ACCOUNT"/>
  <add key="safeChargeMERCHANT_ID" value="MERCHANT_ID_FROM_SANDBOX_ACCOUNT"/>
  <add key="safeChargeMERCHANT_SITE_ID" value="MERCHANT_SITE_ID_FROM_SANDBOX_ACCOUNT"/>

<!-- Application wide Safecharge credentials and endpoints -->
```
4. `dotnet run SafeCharge.sln`
