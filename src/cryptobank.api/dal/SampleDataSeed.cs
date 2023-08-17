using cryptobank.api.dal.news;

namespace cryptobank.api.dal;

public static class SampleDataSeed
{
    public static async Task ApplySamplesAsync(this CryptoBankDbContext dbContext, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
            return;
        
        dbContext.News.Add(new News
        {
            Id = "1EE9D164-883D-4E05-AF8D-1A541A6E5D92",
            Title = "Bitcoin is going to the moon!",
            Content = "Bitcoin is going to the moon! Our analysts predict a price of $100,000 by the end of the year!",
            Date = new DateTime(2021, 10, 1, 15, 45, 0),
            Author = "John Doe"
        });

        dbContext.News.Add(new News
        {
            Id = "80B4F2EA-A9A0-418B-B158-6816CDE7E6CD",
            Title = "Oh no, Bitcoin is crashing!",
            Content = "Oh no, Bitcoin is crashing! This is the end of the world!",
            Date = new DateTime(2022, 3, 5, 18, 5, 0),
            Author = "Jane Doe"
        });

        await dbContext.SaveChangesAsync();
    }
}