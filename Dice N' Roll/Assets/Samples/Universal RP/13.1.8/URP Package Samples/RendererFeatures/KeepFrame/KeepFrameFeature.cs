

//This renderer feature will replicate a "don't clear" behaviour by injecting two passes into the pipeline:
//One pass that copies color at the end of a frame
//Another pass that draws the content of the copied texture at the beginning of a new frame