using System;
using APIs.Services.Interfaces;
using APIs.Utils.Paging;
using BusinessObjects.Models;
using DataAccess.DAO;

namespace APIs.Services
{
    public class AddressService : IAddressService
    {
        public int AddNewAddress(Address address) => new AddressDAO().AddNewAddress(address);

        public async Task<int> DeleteAddressAsync(Guid addressId) => (await new AddressDAO().DeleteAddressAsync(addressId));

        public PagedList<Address> GetAllUserAddress(Guid userId, PagingParams @params)
        {
            return PagedList<Address>.ToPagedList(new AddressDAO().GetAllUserAddress(userId).AsQueryable(), @params.PageNumber, @params.PageSize);
        }
        public int UpdateAddressDefault(Address address) => new AddressDAO().UpdateAddressDefault(address);
    }
}

