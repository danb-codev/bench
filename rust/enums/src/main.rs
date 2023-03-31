//enum IpAddr {
//    V4(u8, u8, u8, u8),
//    V6(String),
//}

//enum Message {
//    Quit,
//    Move { x: i32, y: i32 },
//    Write(String),
//    ChangeColor(i32, i32, i32),
//}

#[derive(Debug)]
enum UsState {
    Alabama,
    Alaska,
}

enum Coin {
    Penny,
    Nickel,
    Dime,
    Quarter(UsState),
}

//fn value_in_cents(coin: Coin) -> u8 {
//    match coin {
//        Coin::Penny => {
//            println!("Lucky penny!");
//            1
//        },
//        Coin::Nickel => 5,
//        Coin::Dime => 10,
//        Coin::Quarter(state) => {
//            println!("State quarter from {:?}!", state);
//            25
//        },
//    }
//}

//fn plus_one(x: Option<i32>) -> Option<i32> {
//    match x {
//        None => None,
//        Some(i) => Some(i + 1),
//    }
//}

fn main() {
    let mut count = 0;
    if let Coin::Quarter(state) = coin {
        println!("State quarter from {state:?}!");
    } else {
        count += 1;
    }

    println!("Count = {count}");
}