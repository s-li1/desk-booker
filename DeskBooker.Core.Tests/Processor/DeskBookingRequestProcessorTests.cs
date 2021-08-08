using System;
using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly DeskBookingRequest _request;
        private readonly Mock<IDeskBookRepository> _deskBookingRepositoryMock;
        private readonly DeskBookerRequestProcessor _processor;

        public DeskBookingRequestProcessorTests()
        {
            //  Set up Method
            _request = new DeskBookingRequest
            {
                FirstName = "Thomas",
                LastName = "Go",
                Email = "thomas.go@email.com",
                Date = new DateTime(2020, 1, 29)
            };

            //  Mock Object
            _deskBookingRepositoryMock = new Mock<IDeskBookRepository>();

            // pass exposed mock object instance to processor constructor

            _processor = new DeskBookerRequestProcessor(_deskBookingRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValues()
        {

            //  Act
            DeskBookingResult result = _processor.BookDesk(_request);

            //  Assert
            Assert.NotNull(_request);
            Assert.Equal(_request.FirstName, result.FirstName);
            Assert.Equal(_request.LastName, result.LastName);
            Assert.Equal(_request.Email, result.Email);
            Assert.Equal(_request.Date, result.Date);
        }

        [Fact]
        public void ShouldThrowNullExceptionIfRequestIsNull()
        {
            //  Act
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

            //  Assert
            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public void ShouldSaveDeskBooking()
        {
            DeskBooking savedDeskBooking = null;
            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>())).Callback<DeskBooking>(deskBooking =>
            {
                savedDeskBooking = deskBooking;
            });

            _processor.BookDesk(_request);

            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

            Assert.NotNull(savedDeskBooking);
            Assert.Equal(_request.FirstName, savedDeskBooking.FirstName);
            Assert.Equal(_request.LastName, savedDeskBooking.LastName);
            Assert.Equal(_request.Email, savedDeskBooking.Email);
            Assert.Equal(_request.Date, savedDeskBooking.Date);
        }
    }
}
