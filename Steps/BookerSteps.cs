using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Text.Json;
using TechTalk.SpecFlow;

namespace ApiTests_LAB3.Steps
{
    [Binding]
    public class BookerSteps
    {
        private RestClient _client;
        private RestResponse _response = null!;
        private string _token = null!;
        private int _bookingId;

        public BookerSteps()
        {
            // Підключаємося до сервера
            _client = new RestClient("https://restful-booker.herokuapp.com");
        }

        [Given(@"I authenticate and get a secure token")]
        public void GivenIAuthenticateAndGetASecureToken()
        {
            var request = new RestRequest("/auth", Method.Post);
            request.AddJsonBody(new { username = "admin", password = "password123" });

            _response = _client.Execute(request);
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var doc = JsonDocument.Parse(_response.Content!);
            _token = doc.RootElement.GetProperty("token").GetString()!;
        }

        [When(@"I create a new booking for ""(.*)"" ""(.*)""")]
        public void WhenICreateANewBookingFor(string firstName, string lastName)
        {
            var request = new RestRequest("/booking", Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(new
            {
                firstname = firstName,
                lastname = lastName,
                totalprice = 150,
                depositpaid = true,
                bookingdates = new { checkin = "2026-10-01", checkout = "2026-10-10" },
                additionalneeds = "Breakfast"
            });

            _response = _client.Execute(request);
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var doc = JsonDocument.Parse(_response.Content!);
            _bookingId = doc.RootElement.GetProperty("bookingid").GetInt32();
        }

        [Then(@"the booking should be created successfully")]
        public void ThenTheBookingShouldBeCreatedSuccessfully()
        {
            Assert.That(_bookingId, Is.GreaterThan(0));
        }

        [When(@"I retrieve the booking details")]
        public void WhenIRetrieveTheBookingDetails()
        {
            var request = new RestRequest($"/booking/{_bookingId}", Method.Get);
            request.AddHeader("Accept", "application/json");
            _response = _client.Execute(request);

            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Then(@"the details should match ""(.*)"" ""(.*)""")]
        public void ThenTheDetailsShouldMatch(string expectedFirstName, string expectedLastName)
        {
            using var doc = JsonDocument.Parse(_response.Content!);
            string actualFirstName = doc.RootElement.GetProperty("firstname").GetString()!;
            string actualLastName = doc.RootElement.GetProperty("lastname").GetString()!;

            Assert.That(actualFirstName, Is.EqualTo(expectedFirstName));
            Assert.That(actualLastName, Is.EqualTo(expectedLastName));
        }

        [When(@"I update the booking checkout date")]
        public void WhenIUpdateTheBookingCheckoutDate()
        {
            var request = new RestRequest($"/booking/{_bookingId}", Method.Patch);
            request.AddHeader("Cookie", $"token={_token}");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(new { bookingdates = new { checkin = "2026-10-01", checkout = "2026-10-15" } });

            _response = _client.Execute(request);
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Then(@"the booking should be updated successfully")]
        public void ThenTheBookingShouldBeUpdatedSuccessfully()
        {
            using var doc = JsonDocument.Parse(_response.Content!);
            string newCheckout = doc.RootElement.GetProperty("bookingdates").GetProperty("checkout").GetString()!;
            Assert.That(newCheckout, Is.EqualTo("2026-10-15"));
        }

        [When(@"I delete the booking")]
        public void WhenIDeleteTheBooking()
        {
            var request = new RestRequest($"/booking/{_bookingId}", Method.Delete);
            request.AddHeader("Cookie", $"token={_token}");

            _response = _client.Execute(request);
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Then(@"the booking should not exist anymore")]
        public void ThenTheBookingShouldNotExistAnymore()
        {
            var request = new RestRequest($"/booking/{_bookingId}", Method.Get);
            var response = _client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}