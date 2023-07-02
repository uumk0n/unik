package classes;
import interfaces.*;
public abstract class Picture implements Drawable, Printable {
    protected int width;
    protected int height;
    protected String author;

    public Picture() {
        this.width = 0;
        this.height = 0;
        this.author = "";
    }

    public Picture(int width, int height, String author) {
        this.width = width;
        this.height = height;
        this.author = author;
    }

    public int getWidth() {
        return width;
    }

    public void setWidth(int width) {
        this.width = width;
    }

    public int getHeight() {
        return height;
    }

    public void setHeight(int height) {
        this.height = height;
    }

    public String getAuthor() {
        return author;
    }

    public void setAuthor(String author) {
        this.author = author;
    }

    @Override
    public String toString() {
        return "Picture{" +
                "width=" + width +
                ", height=" + height +
                ", author='" + author + '\'' +
                '}';
    }
}