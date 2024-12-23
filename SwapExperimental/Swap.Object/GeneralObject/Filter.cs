using GeoCoordinatePortable;
using Swap.Object.Items;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;


namespace Swap.Object.GeneralObjects
{
    public class Filter
    {
        public User FilteredBy { private set; get; }
        public ItemType? TypeOfItem { private set; get; } = null;
        public double? Radius { private set; get; } = null;
        public string City { private set; get; }
        public string Name { private set; get; }
        public Item.ItemCondition? Condition { private set; get; } = null;
        public string Genre { private set; get; }
        public string Author { private set; get; }
        public int? Pages { private set; get; }
        public VideoGame.GamingPlatform? Platform { private set; get; } = null;

        private const string nameParam = "name";
        private const string cityParam = "city";
        private const string genereParam = "gen";
        private const string authorParam = "au";
        private const string pagesParam = "pa";
        private const string uIIParams = "uii";

        public enum ItemType
        {
            Item,
            Book,
            Videogame
        };

        public Filter(User filteredBy,NameValueCollection queryParams)
        {
            FilteredBy = filteredBy;
            TypeOfItem = (ItemType?)SetValue(queryParams, "t");
            Name = queryParams.AllKeys.Contains(nameParam) ? queryParams[nameParam] : null;
            City = queryParams.AllKeys.Contains(cityParam) ? queryParams[cityParam] : null;
            Condition = (Item.ItemCondition?)SetValue(queryParams, "con");
            Genre = queryParams.AllKeys.Contains(genereParam) ? queryParams[genereParam] : null;
            Author = queryParams.AllKeys.Contains(authorParam) ? queryParams[authorParam] : null;
            Pages = SetValue(queryParams, "pa");
            Platform = (VideoGame.GamingPlatform?)SetValue(queryParams, "plat");

            if (queryParams.AllKeys.Contains("rad") && double.TryParse(queryParams["rad"], out double radius))
                Radius = radius;

            if (Platform != null)
                TypeOfItem = ItemType.Videogame;
            else if (Pages != null || Author != null)
                TypeOfItem = ItemType.Book;
        }

        private int? SetValue(NameValueCollection queryParams, string key) 
        {
            if (queryParams.AllKeys.Contains(key))
                if (int.TryParse(queryParams[key], out int value))
                    return value;

            return null;
        }

        public IQueryable<Item> ToQueryable(List<Item> itemsData)
        {
            IQueryable<Item> query = itemsData.AsQueryable();
            query = LoadParameter(query, Name, i => FilterName(i));
            query = LoadParameter(query, Genre, i => i.Genre == Genre);
            query = LoadParameter(query, Condition, i => i.Condition <= Condition);
            query = LoadParameter(query, Radius, i => FilterByDistance(i));
            query = LoadParameter(query, City, i => FilterCity(i));

            if (TypeOfItem == ItemType.Book)
            {
                query = LoadParameter(query, TypeOfItem, i => i is Book);
                query = LoadParameter(query, Author, i => FilterAuthor(i));
            }
            else if (TypeOfItem == ItemType.Videogame)
            {
                query = LoadParameter(query, TypeOfItem, i => i is VideoGame);
                query = LoadParameter(query, Platform, i => i is VideoGame videogame && videogame.Platform == Platform);
            }

            return query;
        }

        private IQueryable<Item> LoadParameter(IQueryable<Item> query, object filterParameter, Func<Item, bool> filterMethod)
        {
            if (filterParameter != null)
                return query.Where(i => filterMethod(i));
            return query;
        }

        private bool FilterAuthor(Item item)
        {
            Book book = (item as Book);
            return item is Book && book.Author != null && Author != null && book.Author.ToLower().Contains(Author.ToLower());
        }

        private bool FilterName(Item item)
        {
            return item.Name != null && Name != null && item.Name.ToLower().Contains(Name.ToLower());
        }

        private bool FilterCity(Item item)
        {
            return null != item?.Owner?.City && null != City && item.Owner.City.ToLower().Contains(City.ToLower());
        }

        private bool FilterByDistance(Item item)
        {
            if (null == Radius)
                return false;

            User userToFilterHisItems = item.Owner;
            if (null == userToFilterHisItems)
                return false;

            GeoCoordinate userToFilterHisItemsCoordinate =
                new GeoCoordinate(userToFilterHisItems.Latitude, userToFilterHisItems.Longitude);

            GeoCoordinate coordinateOfFilteringUser =
                new GeoCoordinate(FilteredBy.Latitude, FilteredBy.Longitude);

            return coordinateOfFilteringUser.GetDistanceTo(userToFilterHisItemsCoordinate) / 1000 <= Radius;
        }
    }
}