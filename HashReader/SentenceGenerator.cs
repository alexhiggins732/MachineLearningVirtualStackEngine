using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashReader
{
    class QuestionGenerator
    {
        public QuestionGenerator()
        {
            var collectionnames = new[] { "number", "day of the week", "month of the year", "hour of the day", "week of the year" };

            var numberAliases = new[] { "number", "integer", "uint", "uint32", "postive integer", "unsigned integer" };

            var dayOfWeek = DayOfWeek.Friday;

            // days in the week| days of the week, weekdays.
            //how many days are in a week, how many weekdays are there. how many days are in days of the week.
            //count the number of days in the week, count the days in the week, count the weekdays,
            
            var countQuestions = new[] { "how many", "count", "length" };

            //1) prime word factorize each word -> days=Days=DAYS 
            //2) destem? delem?
            //3) get id for word
            //4) prime factorization for phrase permuations [how many|count the][number of days in a week][days in the week][week days]

            //5) Instruction factorization => Collection length = {Array =  (arr) = arr.Length; IList =  (lst) = lst.Length; IEnumerable = (en)=> en.Count());
            // ->  crawl text, build indexes,

            //5) test variations permuations map to single instruction or instruction template map->
            //      -> Enum.GetValues(typeof(DayOfWeek)).Count(); 
            //          Resolve the colleciton = DayOfWeek.
            //          how many, count = whe array then array.length, when list then list.Count, when Enumerable  the .Count();


            /*
             
            factor word fractals into common base.
             link base pointers to synonoms/antonums.

            link fractal ids to patterns or Fractal.Fractal type patterns. (lemmification and destemming removes type (noun, adverb, adjective, etc)

            may fractal combination patterns to instruction templates.

            may intruction templates to insutrction templates for various runtime types.
              
            
             */
        }
    }
}
