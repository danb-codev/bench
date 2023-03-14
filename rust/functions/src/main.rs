fn main() {
    let x = plus_one_and_another_one(5);

    println!("The value of x is: {x}");
}

fn plus_one_and_another_one(x: i32) -> i32 {
    let y: i32 = 1;

    x + 1 + y
}