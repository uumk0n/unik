package classes;

public class Painting extends Picture {
    private String style;
    private String paintingTechnique;

    public Painting() {
        super();
        this.style = "";
        this.paintingTechnique = "";
    }

    public Painting(int width, int height, String author, String style, String paintingTechnique) {
        super(width
        , height, author);
        this.style = style;
        this.paintingTechnique = paintingTechnique;
        }
        // Getters and setters
public String getStyle() {
    return style;
}

public void setStyle(String style) {
    this.style = style;
}

public String getPaintingTechnique() {
    return paintingTechnique;
}

public void setPaintingTechnique(String paintingTechnique) {
    this.paintingTechnique = paintingTechnique;
}

// toString method
@Override
public String toString() {
    return "Painting{" +
            "style='" + style + '\'' +
            ", paintingTechnique='" + paintingTechnique + '\'' +
            ", width=" + getWidth() +
            ", height=" + getHeight() +
            ", author='" + getAuthor() + '\'' +
            '}';
}

// Polymorphic method from Drawable interface
@Override
public void draw() {
    System.out.println("Drawing a painting in the style of " + style + ".");
}

// Polymorphic method from Printable interface
@Override
public void print() {
    System.out.println("Printing a painting by " + getAuthor() + ".");
}
}

