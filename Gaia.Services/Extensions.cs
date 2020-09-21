using Axis.Pollux.Common.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gaia.Services
{
    public static class Extensions
    {
        public static async Task<Data[]> GetAll<Data>(this ArrayPageRequest request, Func<ArrayPageRequest, Task<ArrayPage<Data>>> query)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            ArrayPage<Data> dataPage = null;
            var dataList = new List<Data>();
            do
            {
                dataPage = await query.Invoke(request);
                dataList.AddRange(dataPage.Page);
                request = request.NextPage();
            }
            while (dataPage.Page.Length != 0);

            return dataList.ToArray();
        }

        public static ArrayPageRequest NextPage(this ArrayPageRequest request)
        {
            return new ArrayPageRequest
            {
                PageSize = request.PageSize,
                PageIndex = request.PageIndex + 1
            };
        }

        public static ArrayPageRequest PreviousPage(this ArrayPageRequest request)
        {
            return new ArrayPageRequest
            {
                PageSize = request.PageSize,
                PageIndex = request.PageIndex > 0
                    ? request.PageIndex - 1
                    : 0
            };
        }
    }
}
