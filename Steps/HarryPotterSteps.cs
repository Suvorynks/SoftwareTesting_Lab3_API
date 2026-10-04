using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Text.Json;
using TechTalk.SpecFlow;

namespace ApiTests_LAB3.Steps
{
    [Binding]
    public class HarryPotterSteps
    {
        private RestClient _client;
        private RestResponse _response = null!;

        public HarryPotterSteps()
        {
            // 1. Базова адреса сервера (Root URL)
            _client = new RestClient("https://potterapi-fedeperin.vercel.app");
        }

        [Given(@"I connect to the Harry Potter API")]
        public void GivenIConnectToTheHarryPotterAPI()
        {
            // Просто перевіряємо, що клієнт успішно створився
            Assert.That(_client, Is.Not.Null);
        }

        [When(@"I request the list of spells")]
        public void WhenIRequestTheListOfSpells()
        {
            // 2. Додаємо шлях (Endpoint) до українських заклинань: /uk/spells
            // Використовуємо метод GET, бо ми хочемо лише ПРОЧИТАТИ дані, нічого не створюючи
            var request = new RestRequest("/uk/spells", Method.Get);
            _response = _client.Execute(request);
        }

        [Then(@"the API response should be successful")]
        public void ThenTheAPIResponseShouldBeSuccessful()
        {
            // 3. Перевіряємо, що сервер не впав і відповів статусом 200 (OK)
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Then(@"the response should contain a list of spells")]
        public void ThenTheResponseShouldContainAListOfSpells()
        {
            // 4. Розбираємо сирий текст JSON у зручну структуру
            using var doc = JsonDocument.Parse(_response.Content!);

            // Перевіряємо, що нам прийшов масив (список), а не один елемент
            Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Array), "Очікувався список заклинань!");
            Assert.That(doc.RootElement.GetArrayLength(), Is.GreaterThan(0), "Список порожній!");

            // 5. Для краси беремо найперше заклинання з масиву (індекс 0) і виводимо його
            string spellName = doc.RootElement[0].GetProperty("spell").GetString()!;
            string spellUse = doc.RootElement[0].GetProperty("use").GetString()!;

            TestContext.WriteLine($"Успіх! Отримано заклинання українською: {spellName} (Дія: {spellUse})");
        }
    }
}