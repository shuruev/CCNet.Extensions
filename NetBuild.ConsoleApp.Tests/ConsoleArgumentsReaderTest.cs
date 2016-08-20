using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetBuild.ConsoleApp.Tests
{
	[TestClass]
	public class ConsoleArgumentsReaderTest
	{
		[TestMethod]
		public void Cannot_Use_Duplicate_Names()
		{
			var args = new ConsoleArgumentsReader();
			args.Add("item1", "value1");
			args.Invoking(a => args.Add("item1", "value2"))
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Another value was already specified for property name 'item1'.");

			args.Remove("item1").Should().BeTrue();
			args.Add("item1", "value2");
			args.Names.Should().BeEquivalentTo("item1");
			args.GetValue("item1").Should().Be("value2");
		}

		[TestMethod]
		public void Names_Are_Stored_Without_Whitespaces()
		{
			var args = new ConsoleArgumentsReader();
			args.Add("item1   ", "value1");
			args.Names.Should().BeEquivalentTo("item1");
			args.GetValue("item1").Should().Be("value1");

			args.Invoking(a => a.Add(" i t e m 1 ", "value"))
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Another value was already specified for property name 'item1'.");

			args.Remove("   item1").Should().BeTrue();
			args.Add("i\rt\ne\tm\v1", "value2");
			args.Names.Should().BeEquivalentTo("item1");
			args.GetValue(" i t e m 1 ").Should().Be("value2");
		}

		[TestMethod]
		public void Names_Are_Stored_As_Case_Insensitive()
		{
			var args = new ConsoleArgumentsReader();
			args.Add("Item 1", "value1");
			args.Names.Should().BeEquivalentTo("Item1");
			args.GetValue("ITEM 1").Should().Be("value1");

			args.Invoking(a => a.Add("item 1", "value"))
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Another value was already specified for property name 'Item1'.");

			args.Remove("   iTEM   1").Should().BeTrue();
			args.Add("ITEM1", "value2");
			args.Names.Should().BeEquivalentTo("ITEM1");
			args.GetValue("ITEM 1").Should().Be("value2");

			args.Invoking(a => a.Add("item 1", "value"))
				.ShouldThrow<InvalidOperationException>()
				.WithMessage("Another value was already specified for property name 'ITEM1'.");
		}

		[TestMethod]
		public void Names_Cannot_Be_Null()
		{
			var args = new ConsoleArgumentsReader();
			args.Invoking(a => a.Add(null, "value"))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Property name cannot be null.");

			args.Invoking(a => a.GetValue(null))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Property name cannot be null.");

			args.Invoking(a => a.Remove(null))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Property name cannot be null.");
		}

		[TestMethod]
		public void Names_Cannot_Be_Empty_Or_Whitespace()
		{
			var args = new ConsoleArgumentsReader();
			args.Invoking(a => a.Add(String.Empty, "value"))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value '' cannot be used as property name.");

			args.Invoking(a => a.Add("   \t   ", "value"))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value '   \t   ' cannot be used as property name.");

			args.Invoking(a => a.Add("\r\n\t\v", "value"))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value '\r\n\t\v' cannot be used as property name.");

			args.GetValue(String.Empty).Should().BeNull();
			args.Remove(String.Empty).Should().BeFalse();

			args.GetValue("   \t   ").Should().BeNull();
			args.Remove("   \t   ").Should().BeFalse();

			args.GetValue("\r\n\t\v").Should().BeNull();
			args.Remove("\r\n\t\v").Should().BeFalse();
		}

		[TestMethod]
		public void Values_Cannot_Be_Null()
		{
			var args = new ConsoleArgumentsReader();
			args.Invoking(a => a.Add("item", null))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Property value cannot be null.");
		}

		[TestMethod]
		public void Values_Can_Be_Empty_Or_Whitespace()
		{
			var args = new ConsoleArgumentsReader();
			args.Add("item", String.Empty);
			args.Names.Should().BeEquivalentTo("item");
			args.GetValue("item").Should().BeEmpty();

			args = new ConsoleArgumentsReader();
			args.Add("item", "   \t   ");
			args.Names.Should().BeEquivalentTo("item");
			args.GetValue("item").Should().Be("   \t   ");

			args = new ConsoleArgumentsReader();
			args.Add("item", "\r\n\t\v");
			args.Names.Should().BeEquivalentTo("item");
			args.GetValue("item").Should().Be("\r\n\t\v");
		}
	}
}
