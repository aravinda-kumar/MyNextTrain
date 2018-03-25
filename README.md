# MyNextTrain
An app that notifies you when your next train is arriving at the station

The app is built using Xamarin.Forms and MVVM, using Prism as MVVM framework, and DryIOC as the IoC container.

To consume the IrishRail api, I downloaded the XML data, generated the .xsd schema files using xsd.exe and then I generated the classes from the schema using again xsd.exe.

There is a simple integration test project to test the ApiService.

The app uses a number of libraries:
- Xamarin.Forms
- Prism
- RestSharp
- Fody
- xUnit
- Xam.Plugins.Notifier