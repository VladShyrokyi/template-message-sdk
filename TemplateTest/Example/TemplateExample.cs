using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using TemplateLib.Block;
using TemplateLib.Factory;

namespace TemplateTest.Example
{
    [TestFixture]
    public class TemplateExample
    {
        [Test]
        public void Order_example()
        {
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
            var paymentConfiguration = TextBlockFactory.CreateTemplate(
                "Payment method: %[METHOD]%",
                new Dictionary<string, ITextBlock>
                {
                    {"METHOD", TextBlockFactory.CreateText("cash")}
                }
            );
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
            var orders = new List<(string, int, int)>
            {
                ("Chair", 1, 25),
                ("Table", 1, 50),
                ("Wardrobe", 1, 45)
            };
            const int discount = 15;
            const int shippingCost = 25;
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
        }
    }
}
