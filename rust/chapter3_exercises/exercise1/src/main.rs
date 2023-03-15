use std::io;
use std::io::Write;

fn main() {
    let input: f64 = ask_conversion("Celcius");
    let result: f64 = to_fahrenheit(input);

    println!("Celcius ({input}) -> Fahrenheit ({result})");

    let input: f64 = ask_conversion("Fahrenheit");
    let result: f64 = to_celcius(input);

    println!("Fahrenheit ({input}) -> Celcius ({result})");
}

fn ask_conversion(question: &str) -> f64 {
    print!("{question}: ");
    io::stdout().flush().unwrap();

    let mut input = String::new();

    io::stdin()
        .read_line(&mut input)
        .expect("Failed to read line");

    let temperature: f64 = input
        .trim()
        .parse()
        .expect("Input entered was not a number");
        
    temperature
}

fn to_celcius(fahrenheit: f64) -> f64 {
   (fahrenheit - 32.0) * 5.0 / 9.0 
}

fn to_fahrenheit(celcius: f64) -> f64 {
    (celcius * 9.0 / 5.0) + 32.0
}