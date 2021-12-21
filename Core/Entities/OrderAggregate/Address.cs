using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities.OrderAggregate
{
    public class Address //nu derivam din BaseEntity pt ca nu avem tabela separata
    {
        //e field in EntityFramework table, avem neoie de constructor fara parametrii ca sa mearga migrarea
        public Address()
        {
        }

        //constructorul cu parametri ne trebuie pt ca adresa e field si cand vom face o comanda noua vom vrea sa populam adresa in table
        public Address(string firstName, string lastName, string street, string city, string state, string zipCode)
        {
            FirstName = firstName;
            LastName = lastName;
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}