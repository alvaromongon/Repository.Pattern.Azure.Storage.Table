using System;
using System.Text;

namespace Repository.Pattern.Azure.Storage.Table.IntegrationTests.DomainModel
{
    public class DomainModelClass
    {
        public DomainModelClass() : this(Guid.NewGuid().ToString())
        {}

        public DomainModelClass(string aString)
        {
            AString = aString;
            AnotherString = Guid.NewGuid().ToString();

            ABool = true;
            ANullableBool = false;

            // TODO: Not supported yet.
            //ADateTime = DateTime.UtcNow;
            //ANullableDateTime = DateTime.UtcNow;

            // TODO: Not supported yet.
            //ATimeSpan = TimeSpan.FromDays(5);
            //ANullableTimeSpan = TimeSpan.FromDays(7);

            ADateTimeOffset = DateTimeOffset.UtcNow;
            ANullableDateTimeOffset = DateTimeOffset.UtcNow;

            ADouble = 5466.54;
            ANullableDouble = 14.56;

            AGuid = Guid.NewGuid();
            ANullableGuid = Guid.NewGuid();

            AInt = 1145;
            ANullableInt = 986;

            ALong = 9845654;
            ANullableLong = 578965646;

            //// TODO: Not supported yet.
            //ADecimal = 8675.4654m;
            //ANullableDecimal = 24684.465468m;

            AByteArray = Encoding.ASCII.GetBytes("AStringToBeConverted");

            // TODO: Not supported yet.
            //AComplexObject = new ComplexObject();
        }

        public string AString { get; set; }
        public string AnotherString { get; set; }
        public bool ABool { get; set; }
        public bool? ANullableBool { get; set; }

        // TODO: Not supported yet.
        //public DateTime ADateTime { get; set; }
        //public DateTime? ANullableDateTime { get; set; }

        // TODO: Not supported yet.
        //public TimeSpan ATimeSpan { get; set; }
        //public TimeSpan? ANullableTimeSpan { get; set; }

        public DateTimeOffset ADateTimeOffset { get; set; }
        public DateTimeOffset? ANullableDateTimeOffset { get; set; }

        public double ADouble { get; set; }
        public double? ANullableDouble { get; set; }

        public Guid AGuid { get; set; }
        public Guid? ANullableGuid { get; set; }

        public int AInt { get; set; }
        public int? ANullableInt { get; set; }

        public long ALong { get; set; }
        public long? ANullableLong { get; set; }

        // TODO: Not supported yet.
        //public decimal ADecimal { get; set; }
        //public decimal? ANullableDecimal { get; set; }

        public byte[] AByteArray { get; set; }

        // TODO: Not supported yet.
        //public ComplexObject AComplexObject { get; set; }
    }

    [Serializable]
    public class ComplexObject
    {
        public ComplexObject()
        {
            AGuid = Guid.NewGuid();
            AString = Guid.NewGuid().ToString();
            AInt = 654564;
        }

        public Guid AGuid { get; set; }
        public string AString { get; set; }
        public int AInt { get; set; }
    }
}
