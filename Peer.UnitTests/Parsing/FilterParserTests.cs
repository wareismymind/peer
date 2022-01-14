using System;
using Peer.Domain;
using Peer.Domain.Filters;
using Peer.Parsing;
using Xunit;

namespace Peer.UnitTests.Parsing
{
    public class FilterParserTests
    {
        public class ParseFilterOption
        {
            [Fact]
            public void RawStringNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => FilterParser.ParseFilterOption(null));
            }

            [Fact]
            public void RawStringEmpty_ReturnsNotEnoughSections()
            {
                var value = FilterParser.ParseFilterOption(string.Empty);
                ResultAsserts.IsError(value, FilterParseError.NotEnoughSections);
            }

            [Fact]
            public void RawStringHasNoDivider_ReturnsNotEnoughSections()
            {
                var value = FilterParser.ParseFilterOption("authorInsomniak");
                ResultAsserts.IsError(value, FilterParseError.NotEnoughSections);
            }

            [Fact]
            public void RawStringHasSemiColonsInContent_ReturnsRegexFilter()
            {
                var value = FilterParser.ParseFilterOption("author:Insomnia:k");
                ResultAsserts.IsValue(value);
            }

            [Fact]
            public void KeyIsEmpty_ReturnsNoFilterKeySpecified()
            {
                var value = FilterParser.ParseFilterOption(":Insomniak");
                ResultAsserts.IsError(value, FilterParseError.NoFilterKeySpecified);
            }

            [Fact]
            public void KeyNotFound_ReturnsUnknownKey()
            {
                var value = FilterParser.ParseFilterOption("doot:Insomniak");
                ResultAsserts.IsError(value, FilterParseError.UnknownFilterKey);
            }

            [Fact]
            public void FilterValueEmpty_ReturnsFilterContentEmpty()
            {
                var value = FilterParser.ParseFilterOption("author:");
                ResultAsserts.IsError(value, FilterParseError.FilterContentEmpty);
            }

            [Fact]
            public void RegexInvalidForStringKey_ReturnsUnknownMatchValue()
            {
                var value = FilterParser.ParseFilterOption("author:Insom**");
                ResultAsserts.IsError(value, FilterParseError.UnknownMatchValue);
            }

            [Fact]
            public void IntInvalidForIntKey_ReturnsUnknownMatchValue()
            {
                var value = FilterParser.ParseFilterOption("id:waka");
                ResultAsserts.IsError(value, FilterParseError.UnknownMatchValue);
            }

            [Fact]
            public void EnumValueInvalidForEnumKey_ReturnsUnknownMatchValue()
            {
                var value = FilterParser.ParseFilterOption("status:theBreadOne");
                ResultAsserts.IsError(value, FilterParseError.UnknownMatchValue);
            }

            [Fact]
            public void RegexValidForStringKey_ReturnsRegexFilter()
            {
                var value = FilterParser.ParseFilterOption("author:waka");
                ResultAsserts.IsValue(value);
                Assert.IsType<RegexFilter>(value.Value);
            }

            [Fact]
            public void RawStringHasTooManyDividersButNoContent_ReturnsRegexFilter()
            {
                var value = FilterParser.ParseFilterOption("author:Insomniak:::::::::");
                ResultAsserts.IsValue(value);
                Assert.IsType<RegexFilter>(value.Value);
            }

            [Fact]
            public void RawStringHasLeadingOrTrailingWhitespaceInSections_ReturnsRegexFilter()
            {
                var value = FilterParser.ParseFilterOption("author    :Insomniak\t\t\t");
                ResultAsserts.IsValue(value);
                Assert.IsType<RegexFilter>(value.Value);
            }

            [Fact]
            public void IntValidForIntKey_ReturnsActionFilter()
            {
                var value = FilterParser.ParseFilterOption("id:10");
                ResultAsserts.IsValue(value);
                Assert.IsType<ActionFilter>(value.Value);
            }

            [Fact]
            public void EnumValidForEnumKey_ReturnsEnumFilter()
            {
                var value = FilterParser.ParseFilterOption("status:Stale");
                ResultAsserts.IsValue(value);
                Assert.IsType<EnumMatchingFilter<PullRequestStatus>>(value.Value);
            }

            [Fact]
            public void EnumValidCaseDoesntMatch_ReturnsEnumFilter()
            {
                //CN: Ensuring case insensitive parsing
                var value = FilterParser.ParseFilterOption("status:stale");
                ResultAsserts.IsValue(value);
                Assert.IsType<EnumMatchingFilter<PullRequestStatus>>(value.Value);
            }
        }
    }
}
