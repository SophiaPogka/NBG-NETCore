using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TinyBank.Core.Model;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;

namespace TinyBank.Core.Implementation.Services
{
    public class CardService : ICardService
    {
        private readonly Data.TinyBankDbContext _dbContext;

        public CardService(Data.TinyBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public ApiResult<Card> CardPayment(SearchCardOptions options)
        {
            if (options == null) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = $"Null {nameof(options)}"
                };
            }

            if (string.IsNullOrWhiteSpace(options.CardNumber)) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = "CardNumber is empty"
                };
            }

            if (options.ExpirationMonth == 0) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = $"ExpirationMonth for cardNumber {options.CardNumber} is empty"
                };
            }

            if (options.ExpirationYear == 0) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = $"ExpirationYear for cardNumber {options.CardNumber} is empty"
                };
            }

            if (options.Amount == 0) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = "The amount is empty"
                };
            }

            //Εύρεση της κάρτας με βάση το CardNumber
            var card = _dbContext.Set<Card>()
                .Where(c => c.CardNumber == options.CardNumber)
                .Where(c => c.Expiration.Year == options.ExpirationYear)
                .Where(c => c.Expiration.Month == options.ExpirationMonth)
                .SingleOrDefault();

            if (card == null) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.NotFound,
                    ErrorText = $"CardNumber {options.CardNumber} was not found of does not match expiraton month & year"
                };
            }

            if (!card.Active) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = $"The card {options.CardNumber} is not active"
                };
            }

            //Ψάχνω να βρω το 1ο account για την κάρτα αυτή, για να δω εάν έχει υπόλοιπο
            var account = _dbContext.Set<Account>()
                //.Where(c => c.CardNumber == options.CardNumber)
                .Include(c => c.Cards)
                .Where(c => c.Cards.Contains(card))
                .FirstOrDefault();

            if (account == null) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.NotFound,
                    ErrorText = $"No one account was found for the CardNumber {options.CardNumber}"
                };
            }

            if (account.State == Constants.AccountState.Inactive) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = $"The account {account.AccountId} of your card {options.CardNumber} is not active"
                };
            }

            if (account.Balance == 0) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = $"The account balance is zero"
                };
            }

            if (account.Balance < options.Amount) {
                return new ApiResult<Card>() {
                    Code = Constants.ApiResultCode.BadRequest,
                    ErrorText = "The available balance of the account is not enough for the payment"
                };
            }

            //Αν έχουν περάσει όλοι οι έλεγχοι, μειώνουμε το balance της κάρτας με το amount του payment
            account.Balance -= options.Amount;

            _dbContext.SaveChanges();

            return new ApiResult<Card>() {
                Code = Constants.ApiResultCode.Success,
                Data = card
            };
        }
    }
}
