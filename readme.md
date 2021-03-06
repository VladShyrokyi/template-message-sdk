Message template sdk [![Nuget](https://img.shields.io/nuget/v/TemplateLib?style=plastic)](https://www.nuget.org/packages/TemplateLib) [![GitHub release (latest by date)](https://img.shields.io/github/v/release/VladShyrokyi/template-message-sdk)](https://github.com/VladShyrokyi/template-message-sdk)
====

# Overview
A small library for creating templates from blocks using code. The same blocks can be used for different templates. The basic principle of working with the library is to provide a flexible and simple system for writing dynamically generated text.

# Writing a template with variables
The writer's regex is passed a `template` with selectors, a `regex` string to find these selectors, and a `lambda function` to create selectors.
```c#
var writer = new RegexTextWriter(
    "I hold %[ITEM]% in my %[HAND]%",   // Template
    "%\\[([^%,\\s]+)\\]%",              // Regex for search selectors
    s => $"%[{s}]%"                     // Function for create selector
);
var text = writer.ToWriting(new Dictionary<string, string>
{
    {"ITEM", "tea"},
    {"HAND", "left hand"}
});
Assert.AreEqual("I hold tea in my left hand", text);
```
If you do not pass a value to any variable, it is replaced with a `default value`, which can also be passed (by default, this is an empty string).

# Generating template from blocks
Using `TemplateBlockFactory` for creating blocks with `default regex`.

### Default regex
Expression - `%\[([^%,\s]+)\]%`.\
Example - `%[EXAMPLE]%`\
Class - `DefaultRegex`

## Order example
```c#
// Child block 1
var deliveryConfiguration = TextBlockFactory.CreateTemplate(
    "Delivery method: %[METHOD]%\n" +
    "Delivery address: %[ADDRESS]%\n" +
    "Delivery date: %[DATE]%",
    new Dictionary<string, ITextBlock>
    {
        {"METHOD", TextBlockFactory.CreateText("courier")},
        {"ADDRESS", TextBlockFactory.CreateText("Khreschatyk St, 32, Kyiv, 02000")},
        {"DATE", TextBlockFactory.CreateText("08.04.2021")}
    }
);

// Child block 2
var paymentConfiguration = TextBlockFactory.CreateTemplate(
    "Payment method: %[METHOD]%",
    new Dictionary<string, ITextBlock>
    {
        {"METHOD", TextBlockFactory.CreateText("cash")}
    }
);

// Block 1
var orderConfiguration = TextBlockFactory.CreateTemplate(
    "Customer name: %[CUSTOMER_NAME]%\n" +
    "Phone: %[PHONE]%\n" +
    "Email address: %[EMAIL]%\n" +
    "%[DELIVERY_CONFIGURATION]%\n" +
    "%[PAYMENT_CONFIGURATION]%\n" +
    "Comment to order: %[COMMENT]%\n" +
    "Order date: %[ORDER_DATE]%",
    new Dictionary<string, ITextBlock>
    {
        {"CUSTOMER_NAME", TextBlockFactory.CreateText("Vladislav Shirokiy")},
        {"PHONE", TextBlockFactory.CreateText("+8888888888")},
        {"EMAIL", TextBlockFactory.CreateText("vlad16062001@gmail.com")},
        {"DELIVERY_CONFIGURATION", deliveryConfiguration},
        {"PAYMENT_CONFIGURATION", paymentConfiguration},
        {"COMMENT", TextBlockFactory.CreateText("Deliver quickly!")},
        {"ORDER_DATE", TextBlockFactory.CreateText("04.08.2021")}
    }
);

// Dynamic block generation function
var orderFactory = new Func<int, (string, int, int), ITextBlock>((number, tuple) =>
    TextBlockFactory.CreateTemplate("%[NUMBER]%. %[ITEM]% %[COUNT]%x%[PRICE]% $",
        new Dictionary<string, ITextBlock>
        {
            {"NUMBER", TextBlockFactory.CreateText(number.ToString())},
            {"ITEM", TextBlockFactory.CreateText(tuple.Item1)},
            {"COUNT", TextBlockFactory.CreateText(tuple.Item2.ToString())},
            {"PRICE", TextBlockFactory.CreateText(tuple.Item3.ToString())},
        }
    )
);

// Data
var orders = new List<(string, int, int)>
{
    ("Chair", 1, 25),
    ("Table", 1, 50),
    ("Wardrobe", 1, 45)
};
const int discount = 15;
const int shippingCost = 25;

// Result
var block = TextBlockFactory.CreateTemplate(
    "Order №%[ORDER_NUMBER]%\n" +
    "%[ORDER_CONFIGURATION]%\n" +
    "-------\n" +
    "%[ORDERS]%\n" +
    "\n" +
    "%[CHECK]%",
    new Dictionary<string, ITextBlock>
    {
        {"ORDER_NUMBER", TextBlockFactory.CreateText("13")},
        {"ORDER_CONFIGURATION", orderConfiguration},
        {
            "ORDERS", TextBlockFactory.MergeTemplates("\n", orders
                .Select((order, number) => orderFactory(number + 1, order))
                .ToArray()
            )
        },
        {
            "CHECK", TextBlockFactory.CreateTemplate(
                "Discount: %[DISCOUNT]% $\n" +
                "Shipping cost: %[SHIPPING_COST]% $\n" +
                "Total amount: %[TOTAL_AMOUNT]% $",
                new Dictionary<string, ITextBlock>
                {
                    {"DISCOUNT", TextBlockFactory.CreateText(discount.ToString())},
                    {"SHIPPING_COST", TextBlockFactory.CreateText(shippingCost.ToString())},
                    {
                        "TOTAL_AMOUNT", TextBlockFactory.CreateText(
                            (orders.Aggregate(0,
                                    (total, order) => total + order.Item2 * order.Item3) +
                                shippingCost -
                                discount)
                            .ToString()
                        )
                    }
                }
            )
        }
    }
);

// Test
const string result = "Order №13\n" +
    "Customer name: Vladislav Shirokiy\n" +
    "Phone: +8888888888\n" +
    "Email address: vlad16062001@gmail.com\n" +
    "Delivery method: courier\n" +
    "Delivery address: Khreschatyk St, 32, Kyiv, 02000\n" +
    "Delivery date: 08.04.2021\n" +
    "Payment method: cash\n" +
    "Comment to order: Deliver quickly!\n" +
    "Order date: 04.08.2021\n" +
    "-------\n" +
    "1. Chair 1x25 $\n" +
    "2. Table 1x50 $\n" +
    "3. Wardrobe 1x45 $\n" +
    "\n" +
    "Discount: 15 $\n" +
    "Shipping cost: 25 $\n" +
    "Total amount: 130 $";
Assert.AreEqual(block.Write(), result);
```

# How add package to your project?

## Add package Reference
```xml
<PackageReference Include="TemplateLib" Version="1.0.0" />
```
