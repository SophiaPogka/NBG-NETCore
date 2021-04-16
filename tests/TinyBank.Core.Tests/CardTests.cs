using Microsoft.EntityFrameworkCore;

using System.Linq;

using TinyBank.Core.Implementation.Data;
using TinyBank.Core.Model;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;
using Xunit;

namespace TinyBank.Core.Tests
{
    public class CardTests : IClassFixture<TinyBankFixture>
    {
        private readonly TinyBankDbContext _dbContext;
        private readonly ICardService _cards;

        public CardTests(TinyBankFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _cards = fixture.GetService<ICardService>();
        }

        [Fact]
        public void Card_Register_Success()
        {
            var customer = new Customer() {
                Firstname = "Dimitris",
                Lastname = "Pnevmatikos",
                VatNumber = "117008856",
                Email = "dpnevmatikos@codehub.gr",
                IsActive = true
            };

            var account = new Account() {
                Balance = 1000M,
                CurrencyCode = "EUR",
                State = Constants.AccountState.Active,
                AccountId = "GR123456789122"
            };

            customer.Accounts.Add(account);

            var card = new Card() {
                Active = true,
                CardNumber = "4111111111111999",
                CardType = Constants.CardType.Debit
            };

            account.Cards.Add(card);

            _dbContext.Add(customer);
            _dbContext.SaveChanges();

            var customerFromDb = _dbContext.Set<Customer>()
                .Where(c => c.VatNumber == "117008855")
                .Include(c => c.Accounts)
                .ThenInclude(a => a.Cards)
                .SingleOrDefault();

            var customerCard = customerFromDb.Accounts
                .SelectMany(a => a.Cards)
                .Where(c => c.CardNumber == "4111111111111111")
                .SingleOrDefault();

            Assert.NotNull(customerCard);
            Assert.Equal(Constants.CardType.Debit, customerCard.CardType);
            Assert.True(customerCard.Active);
        }


        [Fact]
        public void Card_Payment_Success()
        {
            var options = new SearchCardOptions() {

                CardNumber = "4111111111111999",
                ExpirationYear = 2027,
                ExpirationMonth = 04,
                Amount = 100M
            };

            var card = _cards.CardPayment(options);
            Assert.NotNull(card);
        }
    }
}
