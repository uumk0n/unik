import java.util.ArrayList;

import classes.*;
import interfaces.*;

public class App {
    public static void main(String[] args) throws Exception {

        ArrayList<Picture> pictures = new ArrayList<>();

        pictures.add(new Painting(100, 80, "Leonardo da Vinci", "Renaissance", "Oil on canvas"));
        // pictures.add(new Drawing(100, 80, "John Smith", "Pen and Ink", "Sketch"));
        pictures.add(new Reproduction(60, 40, "Anna Brown", "Van Gogh's Starry Night", "Giclee print"));
        pictures.add(new Landscape(89, 93, "Claude Monet", "Water Lilies"));

        for (Picture picture : pictures) {
            if (picture instanceof Drawable) {
                ((Drawable) picture).draw();
            }

            if (picture instanceof Printable) {
                ((Printable) picture).print();
            }
        }

    }
}
