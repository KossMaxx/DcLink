using System;
using System.Collections.Generic;
using System.Linq;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductTypes
{
    public class ProductType : Mapped
    {
        public IdMap Code { get; }
        public string Name { get; }
        public string FullName { get; }
        public int? MainId { get; }
        public bool IsGroupe { get; }
        public bool Web { get; set; }
        public string TypeNameUkr { get; set; }
        public IEnumerable<ProductTypeCategory> Categories { get; }

        public ProductType(IdMap code, string name, string fullName, bool isGroupe, bool web, string typeNameUkr, int? mainId, bool hasMap, IEnumerable<ProductTypeCategory> categories) : base(hasMap)
        {
            Code = code;
            Name = name;
            FullName = fullName;
            MainId = mainId;
            Categories = categories;
            IsGroupe = isGroupe;
            Web = web;
            TypeNameUkr = typeNameUkr;
        }

        public bool IsNew()
        {
            return !HasMap;
        }

        public bool IsChanged()
        {
            return Code?.ExternalId != null;
        }
    }
}
