using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.ModelManagers
{
    public class TagManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="tagsList">tagsList is a list |1|2|3 where tags with tag id 1, 2 and 3 will be added to the given content.</param>
        public static void SetTagsForContent(int contentId, string tagsList)
        {
            if (string.IsNullOrEmpty(tagsList))
                return;

            var current = TagApi.SelectByContent(contentId).Select(t => t.Id);
            var newList = tagsList.Replace(">", "").Split('|');
            foreach (var tag in current)
            {
                if (!newList.Contains(tag.ToString()))
                {
                    TagApi.UnTag(contentId, tag);
                }
            }
            foreach (var tagId in newList)
            {
                if (!string.IsNullOrEmpty(tagId))
                {
                    TagApi.Tag(contentId, int.Parse(tagId));
                }
            }
        }

        public static Tuple<string, List<TagSuggestion>> GetTagStringForContentAndAvailable(int contentId)
        {
            var available = TagApi.SelectSuggestions();
            var tags = TagApi.SelectByContent(contentId);
            string tagsForContent = "";

            foreach (var tag in tags)
            {
                tagsForContent += "|" + tag.Id + ">";
                var availableItem = available.FirstOrDefault(t => t.Id == tag.Id);
                if (availableItem != null)
                {
                    availableItem.Accepted = true;
                }
            }

            return new Tuple<string, List<TagSuggestion>>(tagsForContent, available);
        }
    }
}
