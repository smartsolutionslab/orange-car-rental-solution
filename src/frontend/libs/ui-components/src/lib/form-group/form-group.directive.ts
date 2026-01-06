import { Directive, input, HostBinding } from "@angular/core";

export type FormGroupLayout = "vertical" | "horizontal" | "inline";
export type FormGroupSpacing = "none" | "sm" | "md" | "lg";

/**
 * Form Group Directive
 *
 * Applies consistent styling to form groups for layout and spacing.
 * Use on container elements that wrap multiple form fields.
 *
 * @example
 * <!-- Vertical layout (default) -->
 * <div ocrFormGroup>
 *   <ocr-input label="First Name" />
 *   <ocr-input label="Last Name" />
 * </div>
 *
 * @example
 * <!-- Horizontal layout -->
 * <div ocrFormGroup layout="horizontal">
 *   <ocr-input label="City" />
 *   <ocr-input label="Postal Code" />
 * </div>
 *
 * @example
 * <!-- Inline layout for search forms -->
 * <div ocrFormGroup layout="inline" spacing="sm">
 *   <ocr-input placeholder="Search..." />
 *   <button type="submit">Search</button>
 * </div>
 */
@Directive({
  selector: "[ocrFormGroup]",
  standalone: true,
})
export class FormGroupDirective {
  /** Layout direction */
  readonly layout = input<FormGroupLayout>("vertical");

  /** Spacing between fields */
  readonly spacing = input<FormGroupSpacing>("md");

  /** Number of columns for grid layout (horizontal only) */
  readonly columns = input<number>(2);

  /** Whether fields should wrap */
  readonly wrap = input(true);

  /** Align items (for horizontal/inline) */
  readonly alignItems = input<"start" | "center" | "end" | "stretch">(
    "stretch",
  );

  @HostBinding("class")
  get hostClasses(): string {
    const classes = ["ocr-form-group"];
    classes.push(`ocr-form-group--${this.layout()}`);
    classes.push(`ocr-form-group--spacing-${this.spacing()}`);
    classes.push(`ocr-form-group--align-${this.alignItems()}`);
    if (this.wrap()) {
      classes.push("ocr-form-group--wrap");
    }
    return classes.join(" ");
  }

  @HostBinding("style.--form-group-columns")
  get columnsStyle(): number {
    return this.columns();
  }

  @HostBinding("style")
  get hostStyles(): Record<string, string> {
    const styles: Record<string, string> = {};

    // Layout styles
    switch (this.layout()) {
      case "vertical":
        styles["display"] = "flex";
        styles["flex-direction"] = "column";
        break;
      case "horizontal":
        styles["display"] = "grid";
        styles["grid-template-columns"] = `repeat(${this.columns()}, 1fr)`;
        break;
      case "inline":
        styles["display"] = "flex";
        styles["flex-direction"] = "row";
        styles["flex-wrap"] = this.wrap() ? "wrap" : "nowrap";
        break;
    }

    // Spacing
    const spacingMap: Record<FormGroupSpacing, string> = {
      none: "0",
      sm: "0.5rem",
      md: "1rem",
      lg: "1.5rem",
    };
    styles["gap"] = spacingMap[this.spacing()];

    // Alignment
    const alignMap: Record<string, string> = {
      start: "flex-start",
      center: "center",
      end: "flex-end",
      stretch: "stretch",
    };
    styles["align-items"] = alignMap[this.alignItems()];

    return styles;
  }
}

/**
 * Form Row Directive
 *
 * Creates a horizontal row within a form group.
 * Useful for placing multiple fields on the same line.
 *
 * @example
 * <div ocrFormGroup>
 *   <div ocrFormRow>
 *     <ocr-input label="First Name" />
 *     <ocr-input label="Last Name" />
 *   </div>
 *   <ocr-input label="Email" />
 * </div>
 */
@Directive({
  selector: "[ocrFormRow]",
  standalone: true,
})
export class FormRowDirective {
  /** Number of columns in the row */
  readonly columns = input<number | "auto">(2);

  /** Gap between fields */
  readonly gap = input<FormGroupSpacing>("md");

  @HostBinding("style")
  get hostStyles(): Record<string, string> {
    const cols = this.columns();
    const gapMap: Record<FormGroupSpacing, string> = {
      none: "0",
      sm: "0.5rem",
      md: "1rem",
      lg: "1.5rem",
    };

    return {
      display: "grid",
      "grid-template-columns":
        cols === "auto"
          ? "repeat(auto-fit, minmax(200px, 1fr))"
          : `repeat(${cols}, 1fr)`,
      gap: gapMap[this.gap()],
      width: "100%",
    };
  }
}

/**
 * Full Width Directive
 *
 * Makes a form field span the full width of a grid layout.
 *
 * @example
 * <div ocrFormGroup layout="horizontal">
 *   <ocr-input label="First Name" />
 *   <ocr-input label="Last Name" />
 *   <ocr-textarea label="Bio" ocrFullWidth />
 * </div>
 */
@Directive({
  selector: "[ocrFullWidth]",
  standalone: true,
})
export class FullWidthDirective {
  @HostBinding("style.grid-column")
  readonly gridColumn = "1 / -1";
}
