using System;

namespace DogMatch.Domain.Infrastructure.DomainExtensions
{
    public static class DomainExtensions
    {
        /// <summary>
        /// uses birthday <see cref="DateTime?"/> to determine current age
        /// of dog in years, or months if dog is younger than one year old
        /// </summary>
        /// <param name="bday">birthday <see cref="DateTime"/></param>
        /// <returns>age <see cref="string"/></returns>
        public static string GetAge(this DateTime? bday)
        {
            DateTime today = DateTime.Today;
            int a = (today.Year * 100 + today.Month) * 100 + today.Day;
            int b = (bday.Value.Year * 100 + bday.Value.Month) * 100 + bday.Value.Day;

            int age = (a - b) / 10000;

            string ageStr;
            // if less than 1 year old then determine age in months
            if (age < 1)
            {
                int m1 = (today.Month - bday.Value.Month);
                int m2 = (today.Year - bday.Value.Year) * 12;
                int months = m1 + m2;

                ageStr = months + " months old";
            }
            else if (age == 1)
            {
                ageStr = age + " year old";
            }
            else
            {
                ageStr = age + " years old";
            }

            return ageStr;
        }

        /// <summary>
        /// extracts dog breed from returned image url from the dog.ceo random dog image api
        /// </summary>
        /// <param name="url">dog.ceo api random dog image url <see cref="string"/></param>
        /// <returns>extracted dog breed <see cref="string"/></returns>
        public static string ExtractBreedFromDogImageUrl(this string url)
        {
            string breed = string.Empty;

            // set char that signifies end of breed in image url
            string endOfBreedString = "/";

            // slice off everything in image url before the breed
            string breedSubstring = url[30..];

            if (!string.IsNullOrWhiteSpace(breedSubstring))
            {
                int charLocation = breedSubstring.IndexOf(endOfBreedString, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    // finish extracting breed from url by slicing off remaining url at first "/"
                    string extractedBreed = breedSubstring[..charLocation];

                    // split into array by "-" character
                    string[] breedArray = extractedBreed.Split("-");

                    // if breed string array length > 1 then re-arrange and capitalize them,
                    // else capitalize and set breed from first array item
                    if (breedArray.Length > 1)
                        breed = $"{breedArray[1].CapitalizeWord()} {breedArray[0].CapitalizeWord()}";
                    else
                        breed = breedArray[0].CapitalizeWord();

                    // fix special known cases
                    if (breed == "Germanshepherd")
                        breed = "German Shepherd";
                    else if (breed == "Stbernard")
                        breed = "Saint Bernard";
                    else if (breed == "Cotondetulear")
                        breed = "Coton de Tulear";
                }
            }

            return breed;
        }

        /// <summary>
        /// capitalizes (upper cases) first character of single word <see cref="string"/>
        /// </summary>
        /// <param name="word">single word <see cref="string"/></param>
        /// <returns>capitalized word <see cref="string"/></returns>
        public static string CapitalizeWord(this string word) =>
            $"{char.ToUpper(word[0])}{word[1..]}";
    }
}
