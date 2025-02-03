using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PrioQ.Presentation.API;
using PrioQ.Application.Interfaces;
using PrioQ.Application.DTOs;

namespace Tests.UnitTests.Controllers
{
    public class ReportsControllerTests
    {
        [Fact]
        public void GetReport_ReturnsOkResult_WithReport()
        {
            // Arrange
            var mockReportUseCase = new Mock<IGetAnalyticsReportUseCase>();

            // Create a dummy report object using the correct DTO properties.
            var dummyReport = new AnalyticsReportDto
            {
                AverageTimeInQueueByPriority = new Dictionary<int, TimeSpan>
                {
                    { 1, TimeSpan.FromMinutes(6) }
                },
                ItemsProcessedByDayOfWeek = new Dictionary<DayOfWeek, int>
                {
                    { DayOfWeek.Monday, 5 }
                },
                TotalItemsProcessed = 10
            };

            // Setup the report use case to return the dummy report for any date range.
            mockReportUseCase
                .Setup(x => x.GenerateReport(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(dummyReport);

            var controller = new ReportsController(mockReportUseCase.Object);

            // Define specific from and to parameters.
            DateTime from = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime to = new DateTime(2025, 1, 8, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var result = controller.GetReport(from, to);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dummyReport, okResult.Value);

            // Verify that GenerateReport was called with the specified parameters.
            mockReportUseCase.Verify(x => x.GenerateReport(from, to), Times.Once);
        }

        [Fact]
        public void GetReport_UsesDefaultDates_WhenParametersAreNull()
        {
            // Arrange
            var mockReportUseCase = new Mock<IGetAnalyticsReportUseCase>();

            var dummyReport = new AnalyticsReportDto
            {
                AverageTimeInQueueByPriority = new Dictionary<int, TimeSpan>
                {
                    { 2, TimeSpan.FromMinutes(8) }
                },
                ItemsProcessedByDayOfWeek = new Dictionary<DayOfWeek, int>
                {
                    { DayOfWeek.Friday, 3 }
                },
                TotalItemsProcessed = 7
            };

            // Setup the report use case to return the dummy report.
            mockReportUseCase
                .Setup(x => x.GenerateReport(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(dummyReport);

            var controller = new ReportsController(mockReportUseCase.Object);

            // Act
            // Call GetReport with null parameters so that defaults are used.
            var result = controller.GetReport(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dummyReport, okResult.Value);

            // Verify that GenerateReport was called with non-null dates.
            mockReportUseCase.Verify(x => x.GenerateReport(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }
    }
}
