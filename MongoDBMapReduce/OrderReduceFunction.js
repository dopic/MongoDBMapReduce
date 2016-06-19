function Reduce(key, values) {

    var result = { Name: "", Total: 0 };

    values.forEach(function (v) {

        if (v.Name)
            result.Name = v.Name;
        else
            result.Total += v.Total;
    });

    return result;
}