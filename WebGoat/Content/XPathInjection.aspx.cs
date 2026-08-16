using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace OWASP.WebGoat.NET
{
    public partial class XPathInjection : System.Web.UI.Page
    {
        // Make into actual lesson
        private string xml = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><sales><salesperson><name>David Palmer</name><city>Portland</city><state>or</state><ssn>123-45-6789</ssn></salesperson><salesperson><name>Jimmy Jones</name><city>San Diego</city><state>ca</state><ssn>555-45-6789</ssn></salesperson><salesperson><name>Tom Anderson</name><city>New York</city><state>ny</state><ssn>444-45-6789</ssn></salesperson><salesperson><name>Billy Moses</name><city>Houston</city><state>tx</state><ssn>333-45-6789</ssn></salesperson></sales>";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["state"] != null)
            {
                FindSalesPerson(Request.QueryString["state"]);
            }
        }

        private sealed class XPathVariableContext : XsltContext
        {
            private readonly XsltArgumentList args;

            public XPathVariableContext(XsltArgumentList args)
            {
                this.args = args;
            }

            public override bool Whitespace => true;
            public override int CompareDocument(string baseUri, string nextbaseUri) => 0;
            public override bool PreserveWhitespace(XPathNavigator node) => true;
            public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes) => null;
            public override IXsltContextVariable ResolveVariable(string prefix, string name)
            {
                return new XPathVariable(args.GetParam(name, string.Empty));
            }
        }

        private sealed class XPathVariable : IXsltContextVariable
        {
            private readonly object value;

            public XPathVariable(object value)
            {
                this.value = value;
            }

            public bool IsLocal => false;
            public bool IsParam => true;
            public XPathResultType VariableType => XPathResultType.Any;
            public object Evaluate(XsltContext xsltContext) => value;
        }

        private void FindSalesPerson(string state)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);

            XPathExpression expression = xDoc.CreateNavigator().Compile("//salesperson[state=$state]");
            XsltArgumentList arguments = new XsltArgumentList();
            arguments.AddParam("state", string.Empty, state);
            expression.SetContext(new XPathVariableContext(arguments));

            XmlNodeList list = xDoc.SelectNodes(expression);
            if (list.Count > 0)
            {

            }

        }
    }
}

