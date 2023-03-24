fn main() {
    let user1 = build_user(String::from("someone@example.com"), String::from("someusername123"));

    let user2 = User {
        email: String::from("another@example.com"),
        ..user1
    };

    //println!("user1 = {user1:#?}");
    println!("user2 = {user2:#?}");

    let black = Color(0, 0, 0);
    let origin = Point(0, 0, 0);

    println!("black = {black:#?}");
    println!("point = {origin:#?}");

    let subject = AlwaysEqual;

    println!("subject = {subject:#?}");
}

fn build_user(email: String, username: String) -> User {
    User {
        active: true,
        username,
        email,
        sign_in_count: 1,
    }
}

#[derive(Debug)]
struct User {
    active: bool,
    username: String,
    email: String,
    sign_in_count: u64,
}

#[derive(Debug)]
struct Color(i32, i32, i32);
#[derive(Debug)]
struct Point(i32, i32, i32);

#[derive(Debug)]
struct AlwaysEqual;