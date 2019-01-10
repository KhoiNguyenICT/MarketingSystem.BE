using System.Collections.Generic;
using System.Linq.Expressions;

namespace MarketingSystem.Core.Yesmarket.Support
{
    internal interface IExpressionCollection : IEnumerable<Expression>
    {
        void Fill();
    }
}