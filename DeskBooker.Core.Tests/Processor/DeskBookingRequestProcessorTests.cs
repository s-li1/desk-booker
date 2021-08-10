﻿using System;
using System.Collections.Generic;
using System.Linq;
using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly DeskBookingRequest _request;
        private readonly List<Desk> _availableDesks;
        private readonly Mock<IDeskBookRepository> _deskBookingRepositoryMock;
        private readonly Mock<IDeskRepository> _deskRepositoryMock;
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

            _availableDesks = new List<Desk> { new Desk { Id = 8 } };

            //  Mock Object
            _deskBookingRepositoryMock = new Mock<IDeskBookRepository>();
            _deskRepositoryMock = new Mock<IDeskRepository>();

            _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_request.Date)).Returns(_availableDesks);

            // pass exposed mock object instance to processor constructor

            _processor = new DeskBookerRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValues()
        {
            DeskBookingResult result = _processor.BookDesk(_request);

            Assert.NotNull(_request);
            Assert.Equal(_request.FirstName, result.FirstName);
            Assert.Equal(_request.LastName, result.LastName);
            Assert.Equal(_request.Email, result.Email);
            Assert.Equal(_request.Date, result.Date);
        }

        [Fact]
        public void ShouldThrowNullExceptionIfRequestIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

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
            Assert.Equal(_availableDesks.First().Id, savedDeskBooking.DeskId);
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
        {
            _availableDesks.Clear();

            _processor.BookDesk(_request);

            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
        }
    }
}
