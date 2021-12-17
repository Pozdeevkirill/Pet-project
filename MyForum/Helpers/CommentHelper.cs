using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.Helpers
{
    public static class CommentHelper
    {
        public static HtmlString CommentView(this IHtmlHelper html, string content)
        {
            return new HtmlString(content);
        }
    }
}
