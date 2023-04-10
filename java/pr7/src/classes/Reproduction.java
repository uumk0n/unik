package classes;

public class Reproduction extends Picture {
    private String originalAuthor;
    private String reproductionTechnique;

    public Reproduction() {
        super();
        this.originalAuthor = "";
        this.reproductionTechnique = "";
    }

    public Reproduction(int width, int height, String author, String originalAuthor, String reproductionTechnique) {
        super(width, height, author);
        this.originalAuthor = originalAuthor;
        this.reproductionTechnique = reproductionTechnique;
    }

    public String getOriginalAuthor() {
        return originalAuthor;
    }

    public void setOriginalAuthor(String originalAuthor) {
        this.originalAuthor = originalAuthor;
    }

    public String getReproductionTechnique() {
        return reproductionTechnique;
    }

    public void setReproductionTechnique(String reproductionTechnique) {
        this.reproductionTechnique = reproductionTechnique;
    }

    @Override
    public String toString() {
        return "Reproduction{" +
                "width=" + width +
                ", height=" + height +
                ", author='" + author + '\'' +
                ", originalAuthor='" + originalAuthor + '\'' +
                ", reproductionTechnique='" + reproductionTechnique + '\'' +
                '}';
    }

    @Override
    public void draw() {
        System.out.println("Drawing a reproduction...");
    }

    @Override
    public void print() {
        System.out.println("Printing a reproduction...");
    }
}

