using System;
using Atom.Toolbox;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetBuild.ConsoleApp.Tests
{
	[TestClass]
	public class ConsoleArgumentsTest
	{
		[TestMethod]
		public void Parsing_Expected_Arguments()
		{
			var args = new ConsoleArguments("mode1", "mode2");
			args.Parse(null);
			args.Names.Should().BeEmpty();
			args.GetValue("Mode 1").Should().BeNull();
			args.GetValue("Mode 2").Should().BeNull();

			args = new ConsoleArguments("mode1", "mode2");
			args.Parse(new[] { "value1" });
			args.Names.Should().BeEquivalentTo("mode1");
			args.GetValue("Mode 1").Should().Be("value1");
			args.GetValue("Mode 2").Should().BeNull();

			args.Parse(new[] { "value1", "value2" });
			args.Names.Should().BeEquivalentTo("mode1", "mode2");
			args.GetValue("Mode 1").Should().Be("value1");
			args.GetValue("Mode 2").Should().Be("value2");

			args.Invoking(a => a.Parse(new[] { "value1", "value2", "value3" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Cannot parse property name from specified value 'value3'.");

			args.Invoking(a => a.Parse(new[] { "value1", "-flag1", "value2" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Cannot parse property name from specified value 'value2'.");

			args.Parse(new[] { "value1", "/item1:x", "-flag1" });
			args.Names.Should().BeEquivalentTo("mode1", "item1", "flag1");
			args.GetValue("Mode 1").Should().Be("value1");
			args.GetValue("Mode 2").Should().BeNull();
			args.GetValue("Item 1").Should().Be("x");
			args.GetValue("Flag 1").Should().Be("True");
		}

		[TestMethod]
		public void Parsing_Properties_And_Flags()
		{
			var args = new ConsoleArguments();
			args.Parse(new[]
			{
				"/item1:value1",
				"-flag1",
				"/item 2:value2",
				"-flag 2",
				"/ ITEM 3 :value3",
				"- FLAG 3"
			});

			args.Names.Should().BeEquivalentTo("item1", "flag1", "item2", "flag2", "ITEM3", "FLAG3");
			args.GetValue("Item 1").Should().Be("value1");
			args.GetValue("Item 2").Should().Be("value2");
			args.GetValue("Item 3").Should().Be("value3");
			args.GetValue("Item 4").Should().BeNull();
			args.GetValue("Flag 1").Should().Be("True");
			args.GetValue("Flag 2").Should().Be("True");
			args.GetValue("Flag 3").Should().Be("True");
			args.GetValue("Flag 4").Should().BeNull();

			args.Parse(new[]
			{
				"/myname:Oleg",
				"/myAge:33",
				"- DEBUG"
			});

			args.Get<string>("MyName").Should().Be("Oleg");
			args.IsNull("MyName").Should().BeFalse();
			args.Get<int>("MyAge").Should().Be(33);
			args.IsNull("MyAge").Should().BeFalse();
			args.Get("MyTimeout", TimeSpan.Zero).Should().Be(TimeSpan.Zero);
			args.IsNull("MyTimeout").Should().BeTrue();
			args.Get<bool>("Debug").Should().BeTrue();
			args.IsNull("Debug").Should().BeFalse();
			args.Get("Release", false).Should().BeFalse();
			args.IsNull("Release").Should().BeTrue();
		}

		[TestMethod]
		public void Invalid_Parsing_Examples()
		{
			var args = new ConsoleArguments();
			args.Parse(new[] { "/a:b" });
			args.GetValue("a").Should().Be("b");

			args.Invoking(a => a.Parse(new[] { " /a:b" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Cannot parse property name from specified value ' /a:b'.");

			args.Invoking(a => a.Parse(new[] { "a:b" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Cannot parse property name from specified value 'a:b'.");

			args.Invoking(a => a.Parse(new[] { "/:b" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value '' cannot be used as property name.");

			args.Invoking(a => a.Parse(new[] { "/:" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value '' cannot be used as property name.");

			args.Invoking(a => a.Parse(new[] { "/ :" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value ' ' cannot be used as property name.");

			args.Invoking(a => a.Parse(new[] { "/::" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value '' cannot be used as property name.");

			args.Parse(new[] { "/a:" });
			args.GetValue("a").Should().Be(String.Empty);

			args.Parse(new[] { "//:b" });
			args.GetValue("/").Should().Be("b");

			args.Parse(new[] { "//::" });
			args.GetValue("/").Should().Be(":");

			args = new ConsoleArguments();
			args.Parse(new[] { "-a:b" });
			args.GetValue("a:b").Should().Be("True");

			args.Invoking(a => a.Parse(new[] { " -a" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Cannot parse property name from specified value ' -a'.");

			args.Invoking(a => a.Parse(new[] { "-" }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value '' cannot be used as property name.");

			args.Invoking(a => a.Parse(new[] { "- " }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Specified value ' ' cannot be used as property name.");
		}

		[TestMethod]
		public void Input_Arguments_Can_Be_Null()
		{
			var args = new ConsoleArguments();
			args.Parse(null);
			args.Names.Should().BeEmpty();
		}

		[TestMethod]
		public void Input_Arguments_Cannot_Contain_Null_Items()
		{
			var args = new ConsoleArguments();
			args.Invoking(a => a.Parse(new[] { "item", null }))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Input arguments should not contain null elements.");
		}

		[TestMethod]
		public void Expected_Arguments_Can_Be_Null()
		{
			var args = new ConsoleArguments();
			args.Names.Should().BeEmpty();

			args = new ConsoleArguments(null);
			args.Names.Should().BeEmpty();
		}

		[TestMethod]
		public void Expected_Arguments_Cannot_Contain_Duplicate_Items()
		{
			this.Invoking(t => new ConsoleArguments("item", "item"))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Expected arguments should not contain duplicate elements.");

			this.Invoking(t => new ConsoleArguments("item 1", "   ITEM1   "))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Expected arguments should not contain duplicate elements.");
		}

		[TestMethod]
		public void Expected_Arguments_Cannot_Contain_Null_Or_Whitespace_Items()
		{
			this.Invoking(t => new ConsoleArguments("item", null))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Expected arguments should not contain null or whitespace elements.");

			this.Invoking(t => new ConsoleArguments("item", String.Empty))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Expected arguments should not contain null or whitespace elements.");

			this.Invoking(t => new ConsoleArguments("item", "   \t   "))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Expected arguments should not contain null or whitespace elements.");

			this.Invoking(t => new ConsoleArguments("item", "\r\n\t\v"))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Expected arguments should not contain null or whitespace elements.");
		}
	}
}
