fn main() {
    const DAYS: [&str; 12] = [
        "first",
        "second",
        "third",
        "fourth",
        "fifth",
        "sixth",
        "seventh",
        "eighth",
        "ninth",
        "tenth",
        "eleventh",
        "twelfth"
    ];
    const GIFTS: [&str; 12] = [
        "A partridge in a pear tree",
        "Two turtle doves",
        "Three french hens",
        "Four calling birds",
        "Five golden rings",
        "Six geese a-laying",
        "Seven swans a-swimming",
        "Eight maids a-milking",
        "Nine ladies dancing",
        "Ten lords a-leaping",
        "Eleven pipers piping",
        "Twelve drummers drumming"
    ];

    for i in  0..DAYS.len() {
        println!("On the {} day of Christmas, my true love gave to me", DAYS[i]);

        for j in (0..i + 1).rev() {
            println!("{}", GIFTS[j]);
        }
        
        println!();
    }
}
