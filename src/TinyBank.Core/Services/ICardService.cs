using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBank.Core.Model;
using TinyBank.Core.Services.Options;

namespace TinyBank.Core.Services
{
    public interface ICardService
    {
        public ApiResult<Card> CardPayment(SearchCardOptions options);

    }
}
