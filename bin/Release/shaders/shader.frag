#version 330 core
struct Material {
    sampler2D diffuse;
    float     shininess;
};
struct Light {
    vec3 position;
    vec3 worldColor;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;
uniform Material material;
uniform vec3 viewPos;

out vec4 color_out;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;


void main()
{
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));

    // Diffuse 
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position-FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));

    // Specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec;

    vec3 result = ambient + diffuse + specular;
    vec4 FragColor = vec4(result, 1.0);

    
    float fogDepth = 60; // Replace the 40 to anything... 40 is also ok.
    vec4 fogColour = vec4(light.worldColor, 1.0); // Choose anything

    float originalZ = gl_FragCoord.z / gl_FragCoord.w;
    originalZ = clamp(originalZ, 0, fogDepth);
    float fog = originalZ / fogDepth ;

    color_out = vec4(mix(FragColor, fogColour, fog)); // Color = the pre-fog colour, with lightning applied.
    
}
