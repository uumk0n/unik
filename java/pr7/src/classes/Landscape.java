package classes;
public class Landscape extends Picture {
    private String location;

    public Landscape() {
        super();
        this.location = "";
    }

    public Landscape(int width, int height, String author, String location) {
        super(width, height, author);
        this.location = location;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    @Override
    public String toString() {
        return "Landscape{" +
                "width=" + width +
                ", height=" + height +
                ", author='" + author + '\'' +
                ", location='" + location + '\'' +
                '}';
    }

    @Override
    public void draw() {
        System.out.println("Drawing a landscape...");
    }

    @Override
    public void print() {
        System.out.println("Printing a landscape...");
    }
}