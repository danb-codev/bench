use std::io;

fn main() {
    //// addition
    //let sum = 5 + 10;

    //println!("sum: {sum}");

    //// subtration
    //let difference = 95.5 - 4.3;
    
    //println!("difference: {difference}");

    //// multiplication
    //let product = 4 * 30;

    //println!("product: {product}");

    //// division
    //let quotient = 56.7 / 32.2;

    //println!("quotient: {quotient}");

    //let truncated = -5 / 3; // Results in -1

    //println!("truncated: {truncated}");

    //// remainder
    //let remainder = 43 % 5;

    //println!("remainder: {remainder}");

    //// boolean
    ////let t = true;

    ////let f: bool = false; // with explicit type annotation

    //// char
    //let c = 'z';
    //let z: char = 'ℤ'; // with explicit type annotation
    //let heart_eyed_cat = '😻';

    //println!("c: {c}, z: {z}, heart_eyed_cat: {heart_eyed_cat}");

    //// tuples
    //let tup: (i32, f64, u8) = (500, 6.4, 1);

    //println!("tup: {tup:?}");

    //let (_x, y, _z) = (500, 6.4, 1);

    //println!("The value of y is: {y}");

    //let x: (i32, f64, u8) = (500, 6.4, 1);

    //let five_hundred = x.0;

    //let six_point_four = x.1;

    //let one = x.2;

    //println!("five_hundred: {five_hundred}, six_point_four: {six_point_four}, one: {one}");

    //let a = [1, 2, 3, 4, 5];

    //println!("a: {a:?}");

    //let first = a[0];

    //println!("first: {first}");

    //let second = a[1];

    //println!("second: {second}");

    //let a: [i32; 5] = [1, 2, 3, 4, 5];
    
    //println!("a: {a:?}");

    //let a = [3; 5];

    //println!("a: {a:?}");

    let a = [1, 2, 3, 4, 5];

    println!("Please enter an array index.");

    let mut index = String::new();

    io::stdin()
        .read_line(&mut index)
        .expect("Failed to read line");

    let index: usize = index
        .trim()
        .parse()
        .expect("Index entered was not a number");

    let element = a[index];

    println!("The value of the element at index {index} is: {element}");
}