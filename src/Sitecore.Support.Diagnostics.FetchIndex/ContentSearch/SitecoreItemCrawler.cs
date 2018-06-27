using System.Reflection;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Sitecore.Support.ContentSearch
{
  public class SitecoreItemCrawler : Sitecore.ContentSearch.SitecoreItemCrawler
  {
    private static readonly MethodInfo GetRootItemMethodInfo =
      typeof(Sitecore.ContentSearch.SitecoreItemCrawler).GetMethod("GetRootItem",
        BindingFlags.Instance | BindingFlags.NonPublic);
    public override int GetContextIndexRanking(IIndexable indexable)
    {
      var sitecoreIndexable = indexable as SitecoreIndexableItem;

      if (sitecoreIndexable == null)
      {
        SearchLog.Log.Info("SITECORE SUPPORT: sitecoreIndexable is NULL. Actual type is " + indexable.GetType());
        return int.MaxValue;
      }

      if (GetRootItemMethodInfo.Invoke(this, new object[0]) == null)
      {
        SearchLog.Log.Info("SITECORE SUPPORT: Root is null. Root: " + this.Root);
        return int.MaxValue;
      }

      Item item = sitecoreIndexable.Item;

      using (new SecurityDisabler())
      {
        using (new SitecoreCachesDisabler())
        {
          int rank = item.Axes.Level - this.RootItem.Axes.Level;

          return rank;
        }
      }
    }
  }
}