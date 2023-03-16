use std::io;
use std::io::Write;
use std::time::Duration;

extern crate stopwatch;

use stopwatch::{Stopwatch};

fn main() {
    print!("Input a number: ");

    let mut input: String = String::new();

    io::stdout().flush().unwrap();
    io::stdin()
        .read_line(&mut input)
        .expect("Failed to read line");
    
    let input = input
        .trim()
        .parse()
        .expect("Input entered was not a number");

    let sw = Stopwatch::start_new();

    let output: i64 = fibonacci(input);

    println!("Fibonacci of {input} is {output}");

    let duration = Duration::from_millis(sw.elapsed_ms().try_into().unwrap());

    println!("{}s", duration.as_secs());
}

fn fibonacci(n: i64) -> i64 {
    match n {
        0 => 0,
        1 => 1,
        _ => fibonacci(n - 1) + fibonacci(n - 2)
    }
}
